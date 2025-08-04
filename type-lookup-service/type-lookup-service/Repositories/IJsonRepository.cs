using Newtonsoft.Json.Linq;

namespace type_lookup_service.Repositories
{
    public interface IJsonRepository
    {
        (JArray jsonArray, string typeName) GetSegmentData(string typeName);
        (JArray jsonArray, string typeName) GetTypeLookupData(string typeName, string primaryKeyName, string primaryKeyValue);
    }
}