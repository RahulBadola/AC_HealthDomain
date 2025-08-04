using System.Collections.Generic;
using System.Threading.Tasks;
using time_track_dshp.Models.Dto;
using time_track_dshp.Models.Dto.Debezium;

namespace time_track_dshp.Services
{
    public interface IDomainMicroService
    {
        public string GetEndpoint(string domainName);

        Task<bool> SendRequest<T>(T obj, string domainName) where T : IBaseEntity;
        Task<bool> SendRequestBulk<T>(IEnumerable<T> obj, string domainName) where T : McBase;
    }
}
