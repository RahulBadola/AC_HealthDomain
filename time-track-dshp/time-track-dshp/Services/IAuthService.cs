using System.Threading.Tasks;

namespace time_track_dshp.Services
{
    public interface IAuthService
    {
        Task<string> GetJwtAsync();
    }
}
