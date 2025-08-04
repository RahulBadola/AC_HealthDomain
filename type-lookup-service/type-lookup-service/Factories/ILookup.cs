using type_lookup_service.Model;

namespace type_lookup_service.Factories
{
    public interface ILookup
    {
        TypeData GetDataList(string typeName);
        TypeData GetDataListById(string typeName, string primaryKey, string primaryKeyValue);
    }
}