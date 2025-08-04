using Dapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace time_track_dshp.Data
{
    public class Repository : IRepository
    {
        private readonly ILogger<Repository> _logger;
        private readonly IDbConnection _db;

        public Repository(ILogger<Repository> logger, IDbConnection db)
        {
            _logger = logger;
            _db = db;
        }

        public async Task<T> QuerySingleOrDefaultAsync<T>(CommandDefinition cmd)
        {
            return await _db.QuerySingleOrDefaultAsync<T>(cmd);
        }

        public async Task<IEnumerable<T>> QueryAsync<T>(CommandDefinition cmd)
        {
            return await _db.QueryAsync<T>(cmd);
        }

        public async Task<int> ExecuteAsync(CommandDefinition cmd)
        {
            return await _db.ExecuteAsync(cmd);
        }

        public async Task<T> QueryFirstOrDefaultAsync<T>(CommandDefinition cmd)
        {
            object results = "";
            try
            {
                results = await _db.QueryFirstOrDefaultAsync<dynamic>(cmd);
                if (results != null)
                    _logger.LogTrace("Error while parsing QuerySingleOrDefaultAsync => " + Environment.NewLine + results.ToString());

                var result = await _db.QueryFirstOrDefaultAsync<T>(cmd);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogTrace("Error while parsing QuerySingleOrDefaultAsync => " + ex.Message + Environment.NewLine + results.ToString());
                throw;
            }
        }
    }
}
