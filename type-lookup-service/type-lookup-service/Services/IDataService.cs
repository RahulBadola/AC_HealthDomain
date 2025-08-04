using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using type_lookup_service.Model;

namespace type_lookup_service.Services
{
    public interface IDataService
    {
        /// <summary>
        /// Get a list of data items
        /// </summary>
        /// <param name="typeNames">A list of names of the Type lists being requested.</param>
        /// <returns></returns>
        IEnumerable<TypeData> GetDataForList(List<string> typeNames);
        JObject GetDataForListMap(List<string> typeNames);
        JObject GetTypeDataById(List<SearchModel> searchModels);

        Task<List<object>> GetLookupDataAsync(SearchModel searchModel);
    }
}