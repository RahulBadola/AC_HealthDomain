using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using time_track_service.Data;
using time_track_service.Model;
using time_track_service.Model.Legacy;
using time_track_service.Utils;
using AssureCare.MedCompass.DataAuthorization.Services;
using time_track_service.Extensions;
using System.Threading;
using time_track_service.Model.ServiceDataObject;

namespace time_track_service.Services
{
    public class MemberTimeTrackingService : IMemberTimeTrackingService
    {
        private readonly ContextLogger<MemberTimeTrackingService> _logger;
        private readonly IFieldAuthorizationService _fieldAuthorizationService;
        private readonly IOperationAuthorizationService _operationAuthorizationService;
        private readonly IRequestContextAccessor _requestContextAccessor;
        private readonly ISyncBackNotificationService _syncBackNotificationService;
        private readonly ITimeTrackRepository _timeTrackRepository;
        private readonly ISharedService _sharedService;
        private readonly IOtherServices _otherServices;
        public MemberTimeTrackingService(ContextLogger<MemberTimeTrackingService> logger,
                                            IFieldAuthorizationService fieldAuthorizationService,
                                            IOperationAuthorizationService operationAuthorizationService,
                                            ITimeTrackRepository timeTrackRepository,
                                            IRequestContextAccessor requestContextAccessor,
                                            ISyncBackNotificationService syncBackNotificationService,
                                            ISharedService sharedService,
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
        public async Task<(DbResponse response, LegacyMemberTimeTrackingsModel data)> GetTimeTrackingsAsync(Guid memberGuid, CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("MemberTimeTrackingService - GetTimeTrackingsAsync Begin , memberGuid:" + memberGuid);

            var result = await _timeTrackRepository.GetTimeTrackByMemberIdAsync(memberGuid);
            try
            {
                if (result.response == DbResponse.Found)
                {
                    var typeLookupTask = _otherServices.GetLookUpDataAsync(new List<string>() {
                                    Constants.ProgramStatusType,
                                    Constants.TimeTrackActivityType,
                                    Constants.TimeTrackSubActivityType,
                                    Constants.ProgramType,
                                    Constants.ServicePlanFundingSourceType,
                                    Constants.ServiceType
                                    });
                    var memberData =  _otherServices.GetMemberDetailAsync(memberGuid);
                    var memberProgramTask = _otherServices.GetMemberProgramsAsync(memberGuid);
                    var taskList = new List<Task>
                    {
                        typeLookupTask,
                        memberData,
                        memberProgramTask
                    };
                    await Task.WhenAll(taskList);
                    _logger.LogDebug($"MemberTimeTrackingService - GetTimeTrackingsAsync Success");

                    var model = TimeTrackHelper.FromTimeTracksToMemberTimeTracksModel(memberGuid, result.data, memberData?.Result, memberProgramTask.Result, typeLookupTask.Result);
                    return (DbResponse.Found, model);
                }
                _logger.LogDebug($"MemberTimeTrackingService - GetTimeTrackingsAsync Failed to retrieve data with MemberId" + memberGuid + "Response" + result.response);
                return (result.response, null);
            }
            catch (Exception ex)
            {
                _logger.LogDebug($"MemberTimeTrackingService - GetTimeTrackingsAsync Exception Response" + ex.Message.GetType());
                return (DbResponse.Error, null);
            }
        }

        public async Task<(DbResponse response, LegacyMemberTimeTrackingModel data)> GetTimeTrackingAsync(Guid id, CancellationToken cancellationToken = default)
        {
            _logger.LogDebug($"MemberTimeTrackingService - GetTimeTrackingAsync Begin -TimeTrackingId:{id}");
            var response = await _timeTrackRepository.GetTimeTrackAsync(id);
            try
            {
                if (response.response == DbResponse.Found)
                {
                    var tasks = new List<Task>();
                    var typeLookupTask = _otherServices.GetLookUpDataAsync(new List<string>() {
                                    Constants.ProgramStatusType,
                                    Constants.SiteConfiguration,});
                    var memberData = _otherServices.GetMemberDetailAsync(response.data.MemberId.Value);
                    var memberInfo = response.data.MemberId.HasValue ? memberData?.Result : new Model.ServiceDataObject.Member();
                    var memberProgramsTask = _otherServices.GetMemberProgramsAsync(response.data.MemberId.Value);
                    tasks.Add(memberData);
                    tasks.Add(memberProgramsTask);
                    tasks.Add(typeLookupTask);
                    await Task.WhenAll(tasks);
                    var model = TimeTrackHelper.FromTimeTrackToMemberTimeTrackModel(response.data, memberInfo, memberProgramsTask.Result, typeLookupTask.Result);
                    _logger.LogDebug($"MemberTimeTrackingService - GetTimeTrackingAsync Success");
                    return (response.response, model);
                }
                _logger.LogDebug($"MemberTimeTrackingService - GetTimeTrackingAsync Failed to retrieve data with TimeTrackingId" + id + ", Response" + response.response);
                return (response.response, null);
            }
            catch (Exception ex)
            {
                _logger.LogDebug($"MemberTimeTrackingService - GetTimeTrackingAsync Exception Response" + ex.Message.GetType());
                return (DbResponse.Error, null);
            }
        }

        public async Task<(DbResponse response, LegacyMemberTimeTrackingModel data)> SaveTimeTrackingAsync(Guid memberGuid, LegacyMemberTimeTrackingModel legacyTimeTrack, CancellationToken cancellationToken = default)
        {
            try
            {
                var webTimeTrack = TimeTrackHelper.FromMemberTimeTrackModelToTimeTrack(legacyTimeTrack);
                webTimeTrack.MemberId = memberGuid;
                webTimeTrack.SecurityUserId = _requestContextAccessor.RequestContext.UserId;
                var timeTrackResult = await _timeTrackRepository.GetTimeTrackAsync(webTimeTrack.Id);
                _logger.LogDebug($"MemberTimeTrackingService - SaveTimeTrackingAsync Begin -memberGuid:{memberGuid}");


                _ = _fieldAuthorizationService.SecureForInsertAsync(legacyTimeTrack, cancellationToken);
                var permitted = await this._operationAuthorizationService.CheckPermissionAsync(webTimeTrack, null, cancellationToken);
                if (!permitted) return (DbResponse.Forbidden, null);

                if (timeTrackResult.response == DbResponse.NotFound)
                {
                    _sharedService.SetInsertDefaults(webTimeTrack);
                    var result = await _timeTrackRepository.InsertTimeTrackAsync(webTimeTrack);
                    if (result == DbResponse.Error)
                    {
                        _logger.LogDebug("MemberTimeTrackingService - Unable to save SaveTimeTrackingAsync");
                        return (DbResponse.Error, null);
                    }
                    if (result == DbResponse.Inserted)
                    {
                        var insertSuccess = await _syncBackNotificationService.SyncBackAsync(SyncBackOperations.Insert, webTimeTrack, null);
                        if (!insertSuccess)
                        {
                            _logger.LogError($"MemberTimeTrackingService SaveTimeTrackingAsync - Failed sync back - Deleting TimeTracking");

                            await _timeTrackRepository.DeleteTimeTrackAsync(webTimeTrack.Id);
                            return (DbResponse.Error, null);
                        }
                    }
                    legacyTimeTrack.Id = webTimeTrack.Id;
                    return (result, legacyTimeTrack);
                }
                else if (timeTrackResult.response == DbResponse.Found)
                {
                    _logger.LogError("MemberTimeTrackingService - Updated Id " + webTimeTrack.Id);
                    var updateResult = await _sharedService.UpdateTimeTrackAsync(webTimeTrack);
                    return (updateResult.response, legacyTimeTrack);
                }
                _logger.LogError($"MemberTimeTrackingService SaveTimeTrackingAsync - Error {timeTrackResult.response} ");
                return (timeTrackResult.response, null);
            }
            catch (Exception ex)
            {
                _logger.LogDebug($"MemberTimeTrackingService - GetTimeTrackingAsync Exception Response" + ex.Message.GetType());
                return (DbResponse.Error, null);
            }
        }
    }
}
