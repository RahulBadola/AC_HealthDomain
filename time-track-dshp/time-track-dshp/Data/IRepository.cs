using Dapper;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace time_track_dshp.Data
{
    public interface IRepository
    {
        public Task<T> QuerySingleOrDefaultAsync<T>(CommandDefinition cmd);
        public Task<IEnumerable<T>> QueryAsync<T>(CommandDefinition cmd);
        public Task<int> ExecuteAsync(CommandDefinition cmd);

    }
}
