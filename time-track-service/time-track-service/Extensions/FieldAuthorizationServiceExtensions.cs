using AssureCare.MedCompass.DataAuthorization.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using time_track_service.Model.Dto;

namespace time_track_service.Extensions
{
    internal static class FieldAuthorizationServiceExtensions
    {
        public static Task<TModel> SecureForInsertAsync<TModel>(
            this IFieldAuthorizationService fieldAuthorizationService,
            TModel obj,
            CancellationToken cancellationToken = default)
            => fieldAuthorizationService.NullifyReadOnlyPropertiesAsync(obj, true, cancellationToken);


        public static async Task<TimeTrack> SecureForUpdateAsync(
            this IFieldAuthorizationService fieldAuthorizationService,
            TimeTrack obj,
            TimeTrack original,
            CancellationToken cancellationToken = default)
        {
            var returnValue = await fieldAuthorizationService.RevertReadOnlyPropertiesAsync(obj, original, cancellationToken);
            return returnValue;
        }
    }
}
