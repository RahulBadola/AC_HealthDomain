using System.Collections.Generic;
using System.Threading.Tasks;
using time_track_dshp.Models.Dto;
using time_track_dshp.Models.Dto.Debezium;

namespace time_track_dshp.Services
{
    public interface IHydrationService
    {
        public Task Hydrate(HydrationRecord record);
        public Task HydrateBulk<T>(IEnumerable<T> records) where T : McBase;
    }
}
