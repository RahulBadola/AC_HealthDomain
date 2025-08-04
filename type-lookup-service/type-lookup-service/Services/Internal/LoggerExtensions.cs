using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using type_lookup_service.Model;

namespace type_lookup_service.Services.Internal
{
    internal static class LoggerExtensions
    {
        public static ILogger QueryRoleOperationsCalled(this ILogger logger, OperationQuery query)
        {
            if (logger is null) return logger;
            if (!logger.IsEnabled(LogLevel.Debug)) return logger;

            Log.QueryRoleOperationsCalled(logger, query?.Objects, query?.SecurityRoleIds, null);

            return logger;
        }

        public static ILogger QueryRoleOperationsCompleted(this ILogger logger, IEnumerable<RoleOperation> roleOperations)
        {
            if (logger is null) return logger;

            if (logger.IsEnabled(LogLevel.Debug))
                Log.QueryRoleOperationsResults(logger, ConvertToJsonString(roleOperations), null);

            if (logger.IsEnabled(LogLevel.Information))
                Log.QueryRoleOperationsCompleted(logger, roleOperations.Count(), null);

            return logger;
        }

        private static readonly Lazy<JsonSerializerSettings> _jsonSerializerSettings = new Lazy<JsonSerializerSettings>(CreateJsonSerializerSettings);

        private static string ConvertToJsonString<T>(T obj)
            => (obj is null) ? "null" : JsonConvert.SerializeObject(obj, _jsonSerializerSettings.Value);

        private static JsonSerializerSettings CreateJsonSerializerSettings()
        {
            var returnValue = JsonConvert.DefaultSettings?.Invoke() ?? new JsonSerializerSettings();

            returnValue.Error = (sender, e) =>
            {
                // ignore serialization errors during logging
                e.ErrorContext.Handled = true;
            };
            returnValue.Formatting = Formatting.None;

            return returnValue;
        }

        private static class Log
        {
#pragma warning disable S3218 // Rename this field to not shadow the outer class' member with the same name.

            public static readonly Action<ILogger, string[], Guid[], Exception> QueryRoleOperationsCalled =
                LoggerMessage.Define<string[], Guid[]>(
                    eventId: 1,
                    logLevel: LogLevel.Information,
                    formatString: $"{nameof(AuthorizationService)}.{nameof(AuthorizationService.QueryRoleOperationsAsync)} called. (objects: {{objects}}, securityRoleIds: {{securityRoleIds}})");

            public static readonly Action<ILogger, int, Exception> QueryRoleOperationsCompleted =
                LoggerMessage.Define<int>(
                    eventId: 2,
                    logLevel: LogLevel.Information,
                    formatString: $"{nameof(AuthorizationService)}.{nameof(AuthorizationService.QueryRoleOperationsAsync)} complete. (resultCount: {{resultCount}}");

            public static readonly Action<ILogger, string, Exception> QueryRoleOperationsResults =
                LoggerMessage.Define<string>(
                    eventId: 3,
                    logLevel: LogLevel.Debug,
                    formatString: $"{nameof(AuthorizationService)}.{nameof(AuthorizationService.QueryRoleOperationsAsync)} query complete. (results: {{results}}");

#pragma warning restore S3218 // Rename this field to not shadow the outer class' member with the same name.
        }
    }
}
