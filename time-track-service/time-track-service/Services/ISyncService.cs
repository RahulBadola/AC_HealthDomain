using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using time_track_service.Model.Dto;

namespace time_track_service.Services
{
    public interface ISyncService
    {
        Task<bool> UpsertAsync(List<TimeTrack> entities, CancellationToken cancellationToken);
    }
}
