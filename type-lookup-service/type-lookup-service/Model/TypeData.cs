using Newtonsoft.Json.Linq;

namespace type_lookup_service.Model
{
    public class TypeData
    {
        public string Name { get; set; }
        public JArray Data { get; set; }
    }
}
