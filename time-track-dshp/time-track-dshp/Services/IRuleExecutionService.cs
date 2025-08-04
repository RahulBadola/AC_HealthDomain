using System.Threading.Tasks;

namespace time_track_dshp.Services
{
    public interface IRuleExecutionService
    {
        public Task<bool> SendRequest(object obj);
    }
}
