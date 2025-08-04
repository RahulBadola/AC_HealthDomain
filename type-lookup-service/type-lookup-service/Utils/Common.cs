using System;
using type_lookup_service.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace type_lookup_service.Utils
{
    public static class Common
    {
        public static FilterDefinition<T> GetActiveFilterDefinition<T>() where T : MedCompassBase
        {
            var builder = Builders<T>.Filter;

            // Filter out voided items
            var filter = !builder.Exists(entity => entity.VoidedOn) | builder.Eq("VoidedOn", BsonNull.Value);

            // Filter out expired items
            filter &= !builder.Exists(entity => entity.ExpirationDate) | builder.Eq("ExpirationDate", BsonNull.Value) | builder.Gt(entity => entity.ExpirationDate, DateTime.UtcNow);

            // Filter out not yet effective items
            filter &= builder.Lte(entity => entity.EffectiveDate, DateTime.UtcNow);

            return filter;
        }
    }
}
