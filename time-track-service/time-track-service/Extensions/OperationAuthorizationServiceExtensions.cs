using AssureCare.MedCompass.DataAuthorization.Models;
using AssureCare.MedCompass.DataAuthorization.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using time_track_service.Model.Dto;

namespace time_track_service.Extensions
{
    internal static class OperationAuthorizationServiceExtensions
    {
        public static async Task<bool> CheckPermissionAsync(
            this IOperationAuthorizationService operationAuthorizationService,
            TimeTrack obj,
            TimeTrack original,
            CancellationToken cancellationToken = default)
        {
            if ((original is null || obj.Id == Guid.Empty) &&
                !await operationAuthorizationService.CheckOperationAsync<TimeTrack>(Operation.Add, obj.SegmentId, cancellationToken))
                return false;

            if (!await operationAuthorizationService.CheckOperationAsync<TimeTrack>(Operation.Update, obj.SegmentId, cancellationToken))
                return false;

            return true;
        }
    }
}
