using Microsoft.Extensions.Logging;
using System;
using type_lookup_service.Model;
using type_lookup_service.Repositories;
using type_lookup_service.Utils;

namespace type_lookup_service.Factories
{
    public class JsonData : ILookup
    {
        private readonly ILogger _logger;
        private readonly IJsonRepository _repo;

        public JsonData(
            IContextLogger<JsonData> logger,
            IJsonRepository repo)
        {
            _repo = repo;
            _logger = logger;
        }

        public TypeData GetDataList(string typeName)
        {

            _logger.LogInformation("JsonData:GetDataList - Method Begins");

            if (string.IsNullOrEmpty(typeName))
            {
                _logger.LogError("JsonData:GetDataList - Caller did not pass in any Type Names.");
                throw new ArgumentNullException(nameof(typeName), "Parameter 'typeName' must have a value.");
            }

            _logger.LogInformation("JsonData:GetDataList - Data does not exist in Cache");

            var data = _repo.GetSegmentData(typeName);

            _logger.LogInformation("JsonData:GetDataList - Method Successfully Completed");

            return new TypeData
            {
                Name = data.typeName,
                Data = data.jsonArray
            };
        }

        public TypeData GetDataListById(string typeName, string primaryKey, string primaryKeyValue)
        {

            _logger.LogInformation("JsonData:GetDataList - Method Begins");

            if (string.IsNullOrEmpty(typeName))
            {
                _logger.LogError("JsonData:GetDataList - Caller did not pass in any Type Names.");
                throw new ArgumentNullException(nameof(typeName), "Parameter 'typeName' must have a value.");
            }

            _logger.LogInformation("JsonData:GetDataList - Data does not exist in Cache");

            var data = _repo.GetTypeLookupData(typeName, primaryKey, primaryKeyValue);

            _logger.LogInformation("JsonData:GetDataList - Method Successfully Completed");

            return new TypeData
            {
                Name = data.typeName,
                Data = data.jsonArray
            };
        }
    }
}
