using Dapper;
using time_track_dshp.Attributes;
using time_track_dshp.Data;
using time_track_dshp.Models.Dto;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace time_track_dshp.Services
{
    public class SqlService : ISqlService
    {
        private readonly ILogger<SqlService> _logger;
        private readonly IRepository _repository;
        private readonly Dictionary<string, string> _dInsertSql = new Dictionary<string, string>();

        public SqlService(ILogger<SqlService> logger, IRepository repository)
        {
            _logger = logger;
            _repository = repository;
            AddInsertSQLStatement(typeof(TimeTrack));
            AddInsertSQLStatement(typeof(TimeTrackActivityMap));
        }

        public void AddInsertSQLStatement(Type type)
        {
            _dInsertSql.Add(type.Name, GetInsertSql(type));
        }

        public string GetInsertSql(Type type)
        {
            var propInfos = type.GetProperties();
            var valueTypes = propInfos.Where(x => x.PropertyType.IsValueType || x.PropertyType.IsPrimitive || x.PropertyType.IsEnum || x.PropertyType.Name.ToLower() == "string");
            var nonCustomAttrTypes = valueTypes.Where(x => !x.CustomAttributes.Any(x => x.AttributeType == typeof(NonSqlPropertyAttribute)));
            var fields = nonCustomAttrTypes.Select(x => x.Name);
            var values = fields.Select(x => $"@{x}");
            return $"INSERT INTO {type.Name} ({string.Join(',', fields)}) Values ({string.Join(',', values)})";
        }

        public async Task<T> GetByIdAsync<T>(Guid id) where T : IBaseEntity
        {
            var type = typeof(T).Name;
            var sql = $"SELECT * FROM {type} WHERE {type}Id = '{id}';";
            var result = await _repository.QuerySingleOrDefaultAsync<T>(new CommandDefinition(sql));
            if (result != null) await SetNames(result);
            return result;
        }

        public async Task<IEnumerable<T>> GetByIdAsync<T>(List<Guid> ids) where T : IBaseEntity
        {
            var type = typeof(T).Name;
            var sql = $"SELECT * FROM {type} WHERE {type}Id IN ({string.Join(",", ids.Select(x => $"'{x}'"))});";
            var result = await _repository.QueryAsync<T>(new CommandDefinition(sql));
            foreach (var obj in result)
            {
                await SetNames(obj);
            }
            return result;
        }

        public async Task InsertAsync<T>(T obj)
        {
            var type = typeof(T).Name;
            if (_dInsertSql.TryGetValue(type, out string sql))
            {
                var result = await _repository.ExecuteAsync(new CommandDefinition(sql, obj));
                _logger.LogTrace($"Insert{type} - affected rows: {result}");
            }
            else
                throw new ArgumentException("Unexpected Type - Cannot find Insert SQL Statement");
        }

        public async Task UpdateAsync<T>(T obj, List<string> fields)
        {
            if (!fields.Any()) return;
            var type = typeof(T).Name;
            var fieldSets = fields.Select(x => $"{x} = @{x}").ToList();
            var sql = $"UPDATE {type} SET {string.Join(',', fieldSets)} WHERE {type}Id = @{type}Id;";
            var result = await _repository.ExecuteAsync(new CommandDefinition(sql, obj));
            _logger.LogDebug($"Update{type} - affected rows: {result}");
        }

        private async Task SetNames(IBaseEntity obj)
        {
            if (obj.GetType() == typeof(SecurityUser)) return;
            
            var isSyncLock = obj is ISyncLockEntity;
            
            var possibleUserIds = new List<Guid?>() { obj.InsertedById, obj.UpdatedById, obj.VoidedById };
            if (isSyncLock) possibleUserIds.Add(((ISyncLockEntity)obj).SyncLockedById);
            var userIds = possibleUserIds.Distinct().Where(x => x != null && x != default(Guid)).Select(x => (Guid)x).ToList();
            var users = await GetByIdAsync<SecurityUser>(userIds);

            obj.InsertedByName = GetName(users.Single(x => x.SecurityUserId == obj.InsertedById));
            obj.UpdatedByName = GetName(users.Single(x => x.SecurityUserId == obj.UpdatedById));
            if (obj.VoidedById != null && obj.VoidedById != default(Guid))
                obj.VoidedByName = GetName(users.Single(x => x.SecurityUserId == obj.VoidedById));
            if (isSyncLock)
            {
                var syncLockObj = (ISyncLockEntity)obj;
                if (syncLockObj.SyncLockedById != null && syncLockObj.SyncLockedById != default(Guid))
                    syncLockObj.SyncLockedByName = GetName(users.Single(x => x.SecurityUserId == syncLockObj.SyncLockedById));
            }

        }

        private string GetName(SecurityUser user)
        {
            if (user != null && user.DisplayName == null)
                return string.Format("{0} {1}", user.FirstName, user.LastName);
            else if (user != null && user.DisplayName != null)
                return user.DisplayName;
            else
                return null;
        }
    }
}
