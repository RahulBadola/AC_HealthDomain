using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using type_lookup_service.Data;
using type_lookup_service.Factories;
using type_lookup_service.Model;
using type_lookup_service.Utils;

namespace type_lookup_service.Services
{
    /// <summary>
    /// Service to lookup Data from appropriate location
    /// </summary>
    public class DataService : IDataService
    {
        private readonly ILogger _logger;
        private readonly ILookup _lookups;
        private readonly IRepository _repository;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="lookups"></param>
        /// <param name="logger"></param>
        public DataService(
            IContextLogger<DataService> logger,
            ILookup lookups,
            IRepository repository)
        {
            _logger = logger;
            _lookups = lookups;
            _repository = repository;
        }

        /// <summary>
        /// Get a list of data items
        /// </summary>
        /// <param name="typeName">The name of the Type list being requested.</param>
        /// <returns></returns>
        public TypeData GetData(string typeName)
        {

            _logger.LogInformation("DataService:GetData - Method Begins");

            var result = _lookups.GetDataList(typeName);

            _logger.LogInformation("DataService:GetData - Method Successfully Completed");

            return result;
        }

        /// <summary>
        /// Get a list of data items
        /// </summary>
        /// <param name="typeNames">A list of names of the Type lists being requested.</param>
        /// <returns></returns>
        public IEnumerable<TypeData> GetDataForList(List<string> typeNames)
        {

            _logger.LogInformation("DataService:GetDataForList - Method Begins");

            if (typeNames.Count > 100)
            {
                _logger.LogError("DataService:GetDataForList - Caller passed in to many Type Names.");

                throw new ArgumentException("Cannot pass more than 100 Type Names to method.");
            }

            var result = new ConcurrentBag<TypeData>();
            Parallel.ForEach(typeNames, type =>
            {
                result.Add(GetData(type));
            });

            _logger.LogInformation("DataService:GetDataForList - Method Successfully Completed");

            return result;
        }

        public JObject GetDataForListMap(List<string> typeNames)
        {

            _logger.LogInformation("DataService:GetDataForList - Method Begins");

            if (typeNames.Count > 100)
            {
                _logger.LogError("DataService:GetDataForList - Caller passed in to many Type Names.");

                throw new ArgumentException("Cannot pass more than 100 Type Names to method.");
            }

            var result = new JObject();

            Parallel.ForEach(typeNames, type =>
            {
                var data = GetData(type);
                lock (result)
                {
                    result.Add(data.Name, data.Data);
                }
            });

            _logger.LogInformation("DataService:GetDataForList - Method Successfully Completed");

            return result;
        }

        public JObject GetTypeDataById(List<SearchModel> searchModels)
        {
            _logger.LogInformation("DataService:GetDataForList - Method Begins");

            if (searchModels.Count > 25)
            {
                _logger.LogError("DataService:GetDataForList - Caller passed in to many Type Names.");

                throw new ArgumentException("Cannot pass more than 25 Type Names to method.");
            }

            var result = new JObject();

            foreach (var model in searchModels)
            {
                var data = _lookups.GetDataListById(model.ObjectName, model.PrimaryKeyName, model.PrimayKeyValue);
                result.Add(data.Name, data.Data);
            }

            _logger.LogInformation("DataService:GetDataForList - Method Successfully Completed");

            return result;
        }

        public async Task<List<object>> GetLookupDataAsync(SearchModel searchModel)
        {
            try
            {
                _logger.LogInformation("CosmosService:getLookupDataAsync - Method Begins");

                searchModel.ObjectName = searchModel.ObjectName.ToUpper();
                var result = await _repository.GetLookupData(searchModel);

                _logger.LogInformation("CosmosService:getLookupDataAsync - Method Ends");

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }

            return null;
        }
    }
}
