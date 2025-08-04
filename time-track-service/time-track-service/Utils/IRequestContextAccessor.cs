using time_track_service.Model;

namespace time_track_service.Utils
{
    public interface IRequestContextAccessor
    {
        RequestContext RequestContext { get; }
    }
}