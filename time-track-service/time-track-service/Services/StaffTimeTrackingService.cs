using Microsoft.Extensions.Logging;
using time_track_service.Model.Dto.Legacy;
using time_track_service.Model;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using time_track_service.Utils;
using time_track_service.Data;
using time_track_service.Model.Dto;
using System.Linq;
using time_track_service.Model.ServiceDataObject;
using AssureCare.MedCompass.DataAuthorization.Services;
using time_track_service.Extensions;
using System.Threading;

namespace time_track_service.Services
{
    public class StaffTimeTrackingService : IStaffTimeTrackingService
    {
        private readonly ContextLogger<StaffTimeTrackingService> _logger;
        private readonly ITimeTrackRepository _timeTrackRepository;
        private readonly IRequestContextAccessor _requestContextAccessor;
        private readonly ISyncBackNotificationService _syncBackNotificationService;
        private readonly ISharedService _sharedService;
        private readonly IOtherServices _otherServices;
        private readonly IFieldAuthorizationService _fieldAuthorizationService;
        private readonly IOperationAuthorizationService _operationAuthorizationService;
        public StaffTimeTrackingService(ContextLogger<StaffTimeTrackingService> logger,
             IFieldAuthorizationService fieldAuthorizationService,
             IOperationAuthorizationService operationAuthorizationService,
             IRequestContextAccessor requestContextAccessor,
             ISyncBackNotificationService syncBackNotificationService,
             ISharedService sharedService, 
             ITimeTrackRepository timeTrackRepository, 
             IOtherServices otherServices)
        {
            _logger = logger;
            _fieldAuthorizationService = fieldAuthorizationService;
            _operationAuthorizationService = operationAuthorizationService;
            _timeTrackRepository = timeTrackRepository;
            _requestContextAccessor = requestContextAccessor;
            _syncBackNotificationService = syncBackNotificationService;
            _sharedService = sharedService;
            _otherServices = otherServices;

        }
        public async Task<(DbResponse response, LegacyTimeTrackingsModel data)> GetTimeTrackingsAsync(Guid staffId, CancellationToken cancellationToken = default)
        {
            LegacyTimeTrackingsModel returnData = new LegacyTimeTrackingsModel();
            var response = await _otherServices.ReadStaffAsync(staffId);
            var securityUserId = response.SecurityUserId;
            var timeTrackingResponse = await _timeTrackRepository.GetTimeTrackBySecurityUserIdAsync(securityUserId);
            if (timeTrackingResponse.response != DbResponse.Found)
            {
                returnData = new LegacyTimeTrackingsModel();
                return (timeTrackingResponse.response, returnData);
            }
            else
            {
                var tasks = new List<Task>();
                var typeLookupTask = _otherServices.GetLookUpDataAsync(new List<string>() { Constants.ProgramStatusType, Constants.TimeTrackActivityType, Constants.TimeTrackSubActivityType });
                var memberTask = _otherServices.GetMemberDetailsAsync(timeTrackingResponse.data.Where(e => e.MemberId.HasValue).Select(x => x.MemberId.Value).ToList());
                tasks.Add(memberTask);
                tasks.Add(typeLookupTask);
                await Task.WhenAll(tasks);
                foreach (var item in timeTrackingResponse.data)
                {
                    var member = memberTask.Result.FirstOrDefault(x => x.Id == item.MemberId);
                    var legacyTimeTrackingsItem = TimeTrackHelper.FromTimeTrackToListItemModel(item, member, typeLookupTask.Result);
                    returnData.Items.Add(legacyTimeTrackingsItem);
                    returnData.StaffGuid = staffId;
                    returnData.TotalTime += item.TotalTime ?? 0;
                    returnData.EndDate = item.EndDate;
                    returnData.StartDate = item.StartDate;
                }
            }
            return (timeTrackingResponse.response, returnData);
        }
        public async Task<(DbResponse response, LegacyTimeTrackingModel data)> GetTimeTrackingAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var response = await _timeTrackRepository.GetTimeTrackAsync(id);
            if (response.response != DbResponse.Found)
            {
                return (response.response, new LegacyTimeTrackingModel());
            }
            var memberData = response.data.MemberId.HasValue ? _otherServices.GetMemberDetailAsync(response.data.MemberId.Value) : null;
            var typeLookupTask = await _otherServices.GetLookUpDataAsync(new List<string>() { Constants.ProgramStatusType, Constants.TimeTrackActivityType, Constants.TimeTrackSubActivityType }) ?? new GenericLookup();
            var memberPrograms = response.data.MemberId.HasValue ? await _otherServices.GetMemberProgramsAsync(response.data.MemberId.Value) : new List<Model.ServiceDataObject.MemberProgram>();
            var legacyTimeTrack = TimeTrackHelper.FromTimeTrackToTimeTrackModel(response.data, memberData?.Result, memberPrograms, typeLookupTask);
            return (response.response, legacyTimeTrack);
        }

        public async Task<(DbResponse response, LegacyTimeTrackingModel data)> SaveTimeTrackingAsync(Guid staffGuid, LegacyTimeTrackingModel legacyTimeTrack, CancellationToken cancellationToken = default)
        {
            DbResponse result;
            legacyTimeTrack.StaffGuid = staffGuid;
            legacyTimeTrack.SecurityUserId = _requestContextAccessor.RequestContext.UserId;
            TimeTrack timeTrack = TimeTrackHelper.FromLegacyTimeTrackToTimeTrack(legacyTimeTrack);
            var staffResponse = await _otherServices.ReadStaffAsync(staffGuid);
            var securityUserId = staffResponse.SecurityUserId;
            timeTrack.SecurityUserId = securityUserId;


            var (response, timeTrackResponse) = legacyTimeTrack.Id != null ? await _timeTrackRepository.GetTimeTrackAsync(legacyTimeTrack.Id.Value) : (DbResponse.NotFound, null);
            bool isUpdate = response == DbResponse.Found && timeTrackResponse != null;

            if (isUpdate)
            {
                var updateResult = await _sharedService.UpdateTimeTrackAsync(timeTrack);
                return (updateResult.response, legacyTimeTrack);
            }
            _ = _fieldAuthorizationService.SecureForInsertAsync(legacyTimeTrack, cancellationToken);
            var permitted = await this._operationAuthorizationService.CheckPermissionAsync(timeTrack, null, cancellationToken);
            if (!permitted) return (DbResponse.Forbidden, null);

            _sharedService.SetInsertDefaults(timeTrack);
            result = await _timeTrackRepository.InsertTimeTrackAsync(timeTrack);
            if (result == DbResponse.Error)
            {
                _logger.LogDebug("SaveTimeTrackingAsync - DB Error on Insert");
                return (DbResponse.Error, null);
            }
            var insertSuccess = await _syncBackNotificationService.SyncBackAsync(SyncBackOperations.Insert, timeTrack, null);
            if (!insertSuccess)
            {
                _logger.LogError("SaveTimeTrackingAsync - Failed sync back - deleting staff timeTrack");

                await _timeTrackRepository.DeleteTimeTrackAsync(timeTrack.Id);
                return (DbResponse.Error, null);
            }
            legacyTimeTrack.Id = timeTrack.Id;
            return (result, legacyTimeTrack);
        }

        public async Task<(DbResponse response, bool data)> IsRequireStartAndEndDatesAsync(CancellationToken cancellationToken = default)
        {
            var result = false;
            string configKey = "TIME_AutoCalcTime";
            var response = await _otherServices.GetLookUpDataAsync(new List<string>() { Constants.SiteConfiguration });
            if (response != null)
            {
                var activeConfigItem = response.siteConfiguration.FirstOrDefault(x => x.activeFlag == Constants.ActiveFlag && x.configKey == configKey);
                var configValue = activeConfigItem != null ? activeConfigItem.configValue : "";
                if (configValue != string.Empty && configValue == "Yes")
                {
                    result = true;
                }
            }
            return (DbResponse.Found, result);
        }
        public async Task<(DbResponse response, int data)> SelectServiceUnitMaxValueAsync(CancellationToken cancellationToken = default)
        {
            var result = 9999;
            string configKey = "TT_ServiceUnitMax";
            var response = await _otherServices.GetLookUpDataAsync(new List<string>() { Constants.SiteConfiguration });
            if (response != null)
            {
                var activeConfigItem = response.siteConfiguration.FirstOrDefault(x => x.activeFlag == Constants.ActiveFlag && x.configKey == configKey);
                var configValue = activeConfigItem != null ? activeConfigItem.configValue : "";
                if (configValue != string.Empty && int.TryParse(configValue, out result))
                {
                    result = int.Parse(configValue);
                }
            }
            return (DbResponse.Found, result);

        }
    }
}
