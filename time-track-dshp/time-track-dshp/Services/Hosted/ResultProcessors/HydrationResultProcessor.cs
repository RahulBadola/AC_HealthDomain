using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using time_track_dshp.Models.Dto.Debezium;

namespace time_track_dshp.Services.Hosted.ResultProcessors
{
    public class HydrationResultProcessor : IBulkResultProcessor
    {
        public HydrationResultProcessor(ILogger<HydrationMessageConsumerService> logger, IHydrationService hydrationService)
        {
            _logger = logger;
            _hydrationService = hydrationService;
        }

        public async Task ProcessBulkResult(IEnumerable<ConsumeResult<string, string>> consumeResults)
        {
            try
            {
                if (consumeResults.Any())
                {
                    var hydrationTables = consumeResults
                        .Select(x => JObject.Parse(x.Message.Key).Root.First.Path)
                        .Distinct()
                        .ToList();

                    var timetrack = new List<TimeTrack>();
                    var timetrackactivitymap = new List<TimeTrackActivityMap>();

                    foreach (var table in hydrationTables)
                    {
                        switch (table)
                        {
                            case string tableName when Regex.IsMatch(tableName, @"\bTimeTrackId\b"):
                                timetrack = consumeResults
                                    .Where(x => JObject.Parse(x.Message.Key).Root.First.Path == table)
                                    .Select(x => DeserializeMessageValue<TimeTrack>(x.Message.Value))
                                    .ToList();
                                break;
                            case string tableName when Regex.IsMatch(tableName, @"\bTimeTrackActivityMapId\b"):
                                timetrackactivitymap = consumeResults
                                    .Where(x => JObject.Parse(x.Message.Key).Root.First.Path == table)
                                    .Select(x => DeserializeMessageValue<TimeTrackActivityMap>(x.Message.Value))
                                    .ToList();
                                break;
                            default:
                                _logger.LogError($"Cannot find suitable deserialize method for {table}");
                                break;
                        }
                    }
                    if (timetrack.Any()) await _hydrationService.HydrateBulk(timetrack);
                    if (timetrackactivitymap.Any()) await _hydrationService.HydrateBulk(timetrackactivitymap);

                    _logger.LogDebug($"ProcessResult completed");
                }
                else
                {
                    _logger.LogDebug("No records available to process");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to process message: {JsonConvert.SerializeObject(consumeResults)} --- Error: {ex}");
            }
        }

        private readonly ILogger<HydrationMessageConsumerService> _logger;
        private readonly IHydrationService _hydrationService;

        private static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings
        {
            MissingMemberHandling = MissingMemberHandling.Error
        };

        private static T DeserializeMessageValue<T>(string value)
            => JsonConvert.DeserializeObject<T>(value, JsonSerializerSettings);
    }
}
