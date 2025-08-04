using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using time_track_dshp.Models.Configuration;
using time_track_dshp.Models.Dto;
using time_track_dshp.Models.Dto.Debezium;

namespace time_track_dshp.Services
{
    public class HydrationService : IHydrationService
    {
        private readonly ILogger<HydrationService> _logger;
        private readonly ISqlService _sqlService;
        private readonly IDomainMicroService _domainMicroService;
        private readonly DomainConfiguration _domainConfiguration;

        public HydrationService(ILogger<HydrationService> logger, IDomainMicroService domainMicroService, ISqlService sqlService, DomainConfiguration domainConfiguration)
        {
            _logger = logger;
            _sqlService = sqlService;
            _domainMicroService = domainMicroService;
            _domainConfiguration = domainConfiguration;
        }

        public async Task Hydrate(HydrationRecord record)
        {
            switch (record.DomainName.ToLower())
            {
                case "timetrack":
                    await Hydrate<Models.Dto.TimeTrack>(record);
                    break;
                case "timetrackactivitymap":
                    await Hydrate<Models.Dto.TimeTrackActivityMap>(record);
                    break;
                default:
                    _logger.LogDebug("Unknown domain type from record.");
                    break;
            }
        }

        public async Task Hydrate<T>(HydrationRecord record) where T : IBaseEntity
        {
            _logger.LogDebug($"Retrieving records from SQL Server ({record.DomainName})");
            var obj = await _sqlService.GetByIdAsync<T>(Guid.Parse(record.PrimaryKey));
            obj.TenantId = _domainConfiguration.TenantId.ToString();
            _logger.LogDebug($"Sending records to the MicroService ({record.DomainName})");
            var endpoint = _domainMicroService.GetEndpoint(record.DomainName);
            var result = await _domainMicroService.SendRequest(obj, endpoint) ? "Success" : "Failed";
            _logger.LogDebug($"The MicroService response was: {result}");

            if (record.IsLastItem)
            {
                _logger.LogInformation("The last records has been processed");
                NotifyGateway();
            }
        }
        public async Task HydrateBulk<T>(IEnumerable<T> records) where T : McBase
        {
            records.ToList().ForEach(x => x.TenantId = _domainConfiguration.TenantId.ToString());

            _logger.LogDebug($"Sending records to the MicroService (Member Contact Service)");
            var endpoint = _domainMicroService.GetEndpoint(typeof(T).Name.ToLower());

            var result = await _domainMicroService.SendRequestBulk(records, endpoint) ? "Success" : "Failed";
            _logger.LogDebug($"The MicroService response was: {result}");
        }
        public void NotifyGateway()
        {
            _logger.LogInformation("Notifying Gateway");
            //Notify gateway/reverse-proxy that the service is hydrated and ready
        }

        private Dictionary<string, string> ParseKeys(string primaryKey)
        {
            return JsonSerializer.Deserialize<Dictionary<string, string>>(primaryKey);
        }
    }
}
