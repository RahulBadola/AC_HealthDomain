using Avro;
using Avro.Generic;
using time_track_dshp.Extensions;
using time_track_dshp.Models.Configuration;
using time_track_dshp.Models.Dto;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace time_track_dshp.Services
{
    public class SyncBackService : ISyncBackService
    {
        private readonly ILogger<SyncBackService> _logger;
        private readonly ISyncBackMessageProducerService _messageProducerService;
        private readonly MessagingConfiguration _messagingConfiguration;
        private readonly ISqlService _sqlService;
        private readonly IRuleExecutionService _ruleExecutionService;

        public SyncBackService(ILogger<SyncBackService> logger, ISyncBackMessageProducerService messageProducerService, 
            ISqlService sqlService, MessagingConfiguration messagingConfiguration, IRuleExecutionService ruleExecutionService)
        {
            _logger = logger;
            _messageProducerService = messageProducerService;
            _sqlService = sqlService;
            _messagingConfiguration = messagingConfiguration;
            _ruleExecutionService = ruleExecutionService;
        }
        
        public async Task PublishSyncBackRecordAsync(string objectType, string objectJson)
        {
            _logger.LogDebug("PublishSyncBackRecord");
            var schema = (RecordSchema)Schema.Parse(File.ReadAllText(_messagingConfiguration.Kafka.SyncBackProducerSchemaFileLocation));
            var record = new GenericRecord(schema);
            record.Add("ObjectType", objectType);
            record.Add("ObjectJson", objectJson);
            await _messageProducerService.ProduceMessageAsync(record);
            _logger.LogInformation($"SyncBack Message for type: {objectType}");
            _logger.LogDebug($"SyncBack Object: {objectJson}");
        }

        public async Task SyncBackToSqlServerAsync(SyncBackRecord record)
        {

            switch (record.ObjectType.ToLower())
            {
                case "timetrack":
                    var timeTrackChangeSet = JsonConvert.DeserializeObject<ChangeSet<TimeTrack>>(record.ObjectJson);
                    await SyncBackTimeTrackAsync(timeTrackChangeSet.New);
                    _logger.LogDebug("SyncBack Complete.");
                    await _ruleExecutionService.SendRequest(ApplyRulesRequest<TimeTrack>.NewRequest(timeTrackChangeSet));
                    _logger.LogDebug("Rule Execution Call Complete.");
                    break;
                case "timetrackactivitymap":
                    var timeTrackActivityMapChangeSet = JsonConvert.DeserializeObject<ChangeSet<TimeTrackActivityMap>>(record.ObjectJson);
                    await SyncBackTimeTrackActivityMapAsync(timeTrackActivityMapChangeSet.New);
                    _logger.LogDebug("SyncBack Complete.");
                    await _ruleExecutionService.SendRequest(ApplyRulesRequest<TimeTrackActivityMap>.NewRequest(timeTrackActivityMapChangeSet));
                    _logger.LogDebug("Rule Execution Call Complete.");
                    break;
                default:
                    _logger.LogError($"SyncBack failed for unknown object type: {record.ObjectType}");
                    break;
            }
        }
        
        public async Task SyncBackTimeTrackAsync(TimeTrack timeTrack, bool isInsert = false)
        {
            TimeTrack result = await _sqlService.GetByIdAsync<TimeTrack>(timeTrack.TimeTrackId);
                        
            if (result == null)
            {
                await _sqlService.InsertAsync(timeTrack);
            }
            else
            {
                var fields = result.UpdatedFields(timeTrack);
                await _sqlService.UpdateAsync(timeTrack, fields);
            }
            _logger.LogDebug($"SyncBackTimeTrackAsync complete");
        }
        
        public async Task SyncBackTimeTrackActivityMapAsync(TimeTrackActivityMap timeTrackActivityMap, bool isInsert = false)
        {
            TimeTrackActivityMap result = await _sqlService.GetByIdAsync<TimeTrackActivityMap>(timeTrackActivityMap.TimeTrackActivityMapId);
                        
            if (result == null)
            {
                await _sqlService.InsertAsync(timeTrackActivityMap);
            }
            else
            {
                var fields = result.UpdatedFields(timeTrackActivityMap);
                await _sqlService.UpdateAsync(timeTrackActivityMap, fields);
            }
            _logger.LogDebug($"SyncBackTimeTrackActivityMapAsync complete");
        }
    }
}
