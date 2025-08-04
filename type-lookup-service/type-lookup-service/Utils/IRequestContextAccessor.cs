using type_lookup_service.Model;

namespace type_lookup_service.Utils
{
    public interface IRequestContextAccessor
    {
        RequestContext RequestContext { get; }
    }
}