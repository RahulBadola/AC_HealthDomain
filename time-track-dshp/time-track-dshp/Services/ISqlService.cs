using time_track_dshp.Models.Dto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace time_track_dshp.Services
{
    public interface ISqlService
    {
        void AddInsertSQLStatement(Type type);
        public Task<T> GetByIdAsync<T>(Guid id) where T : IBaseEntity;
        public Task<IEnumerable<T>> GetByIdAsync<T>(List<Guid> ids) where T : IBaseEntity;
        public Task InsertAsync<T>(T obj);
        public Task UpdateAsync<T>(T obj, List<string> fields);
    }
}
