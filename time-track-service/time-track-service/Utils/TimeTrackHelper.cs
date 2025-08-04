using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using Newtonsoft.Json.Linq;
using time_track_service.Model.Dto;
using time_track_service.Model.Dto.Legacy;
using time_track_service.Model.Legacy;
using time_track_service.Model.ServiceDataObject;

namespace time_track_service.Utils
{
    public static class TimeTrackHelper
    {
        public static LegacyTimeTrackingsItemModel FromTimeTrackToListItemModel(TimeTrack timeTrack, Member member, GenericLookup genericLookup)
        {
            var legacyTimeTrackingsItemModel = new LegacyTimeTrackingsItemModel();
            legacyTimeTrackingsItemModel.Id = timeTrack.Id;
            legacyTimeTrackingsItemModel.Date = timeTrack.StartDate.Date;
            var activity = GetTypeKey(genericLookup.TimeTrackActivityType, timeTrack.TimeTrackActivityTypeKey);
            legacyTimeTrackingsItemModel.Activity = activity != null ? activity.typeDescription : timeTrack.TimeTrackActivityTypeKey;
            var subactivity = !string.IsNullOrEmpty(timeTrack.TimeTrackSubActivityTypeKey) ? GetTypeKey(genericLookup.TimeTrackSubActivityType, timeTrack.TimeTrackSubActivityTypeKey) : null;
            legacyTimeTrackingsItemModel.SubActivity = subactivity != null ? subactivity.typeDescription : timeTrack.TimeTrackSubActivityTypeKey;
            legacyTimeTrackingsItemModel.Member = member != null ? string.Format("{0} {1}", member.FirstName, member.LastName) : null;
            var user = !string.IsNullOrEmpty(timeTrack.InsertedByName) ? timeTrack.InsertedByName : null;
            legacyTimeTrackingsItemModel.InsertedByName = user;
            legacyTimeTrackingsItemModel.ServiceUnits = timeTrack.ServiceUnits;
            legacyTimeTrackingsItemModel.TotalTime = timeTrack.TotalTime;
            legacyTimeTrackingsItemModel.FundingSource = timeTrack.ServicePlanFundingSourceTypeKey;

            return legacyTimeTrackingsItemModel;
        }
        public static LegacyTimeTrackingModel FromTimeTrackToTimeTrackModel(TimeTrack timeTrack, Member member, IEnumerable<MemberProgram> memberPrograms, GenericLookup genericLookup)
        {
            var legacyTimeTrackingModel = new LegacyTimeTrackingModel
            {
                Activity = timeTrack.TimeTrackActivityTypeKey,
                AdditionalComments = timeTrack.Comment,
                StartDate = timeTrack.StartDate,
                EndDate = timeTrack.EndDate,
                FundingSource = timeTrack.ServicePlanFundingSourceTypeKey,
                Id = timeTrack.Id,
                MemberId = timeTrack.MemberId,
                MemberProgramId = timeTrack.MemberProgramId,
                ProgramType = timeTrack.ProgramTypeKey,
                Reason = timeTrack.Reason,
                Service = timeTrack.ServiceTypeKey,
                ServiceUnits = timeTrack.ServiceUnits,
                SecurityUserId = timeTrack.SecurityUserId.Value,
                StartTime = timeTrack.StartDate.ToString(),
                EndTime = timeTrack.EndDate.ToString(),
                SubActivity = timeTrack.TimeTrackSubActivityTypeKey,
                TotalTime = timeTrack.TotalTime,
                TravelDuration = timeTrack.TravelDuration,
                TravelMiles = timeTrack.TravelMiles,
                VolunteerDriver = timeTrack.VolunteerDriver,
                InsertedByName = timeTrack.InsertedByName,
                InsertedOn = timeTrack.InsertedOn,
                UpdatedByName = timeTrack.UpdatedByName,
                UpdatedOn = timeTrack.UpdatedOn,
                SegmentId = timeTrack.SegmentId,
                Member = member != null ? string.Format("{0} {1}", member.FirstName, member.LastName) : ""
            };

            legacyTimeTrackingModel.Case = GetCaseDescription(memberPrograms, timeTrack.MemberProgramId, genericLookup.ProgramStatusType);

            return legacyTimeTrackingModel;
        }
        
        public static LegacyMemberTimeTrackingModel FromTimeTrackToMemberTimeTrackModel(TimeTrack timeTrack, Member member, List<MemberProgram> memberPrograms, GenericLookup lookup)
        {

            LegacyMemberTimeTrackingModel legacyTime = new LegacyMemberTimeTrackingModel
            {
                Id = timeTrack.Id,
                AdditionalComments = timeTrack.Comment,
                Case = GetCaseDescription(memberPrograms, member.Id, lookup.ProgramStatusType),
                MemberProgramGuid = timeTrack.MemberProgramId,
                StartDate = timeTrack.StartDate,
                EndDate = timeTrack.EndDate,
                FundingSource = timeTrack.ServicePlanFundingSourceTypeKey,
                ProgramType = timeTrack.ProgramTypeKey,
                Reason = timeTrack.Reason,
                Member = string.Format("{0} {1}", member?.FirstName, member?.LastName),
                Service = timeTrack.ServiceTypeKey,
                ServiceUnits = timeTrack.ServiceUnits,
                Activity = timeTrack.TimeTrackActivityTypeKey,
                SubActivity = timeTrack.TimeTrackSubActivityTypeKey,
                TotalTime = timeTrack.TotalTime,
                TravelDuration = timeTrack.TravelDuration,
                TravelMiles = timeTrack.TravelMiles,
                VolunteerDriver = timeTrack.VolunteerDriver,
                MemberGuid = timeTrack.MemberId,
                CareProgramRequired = IsCareProgramRequired(lookup.siteConfiguration),
                InsertedByName = timeTrack.InsertedByName,
                InsertedOn = timeTrack.InsertedOn,
                SegmentId = timeTrack.SegmentId,
                UpdatedByName = timeTrack.UpdatedByName,
                UpdatedOn = timeTrack.UpdatedOn,
                VoidedReasonId = timeTrack.VoidedReasonId
            };

            if (memberPrograms != null && memberPrograms.Any())
            {
                legacyTime.CaseList = GetMemberCaseModelById(memberPrograms, lookup);
            }

            if (timeTrack.Udf != null)
            {
                legacyTime.Udf = JObject.Parse(timeTrack.Udf.ToString());
            }
            return legacyTime;
        }

        public static TimeTrack FromLegacyTimeTrackToTimeTrack(LegacyTimeTrackingModel legacyTimeTrack)
        {
            return new TimeTrack()
            {
                Id = legacyTimeTrack.Id ?? Guid.NewGuid(),
                SecurityUserId = legacyTimeTrack.SecurityUserId,
                EndDate = legacyTimeTrack.EndDate.Value,
                MemberProgramId = legacyTimeTrack.MemberProgramId,
                TimeTrackActivityTypeKey = legacyTimeTrack.Activity,
                ServiceTypeKey = legacyTimeTrack.Service,
                ProgramTypeKey = legacyTimeTrack.ProgramType,
                ServicePlanFundingSourceTypeKey = legacyTimeTrack.FundingSource,
                TimeTrackSubActivityTypeKey = legacyTimeTrack.SubActivity,
                StartDate = legacyTimeTrack.StartDate.Value,
                MemberId = legacyTimeTrack.MemberId,
                TotalTime = legacyTimeTrack.TotalTime ,
                VolunteerDriver = legacyTimeTrack.VolunteerDriver ?? false,
                ServiceUnits = legacyTimeTrack.ServiceUnits,
                TravelMiles = legacyTimeTrack.TravelMiles,
                TravelDuration = legacyTimeTrack.TravelDuration,
                Reason = legacyTimeTrack.Reason,
                Comment = legacyTimeTrack.AdditionalComments,
                InsertedByName = legacyTimeTrack.InsertedByName,
                InsertedOn = legacyTimeTrack.InsertedOn,
                UpdatedByName = legacyTimeTrack.UpdatedByName,
                UpdatedOn = legacyTimeTrack.UpdatedOn,
                SegmentId = legacyTimeTrack.SegmentId
            };
        }
        
        public static LegacyMemberTimeTrackingsModel FromTimeTracksToMemberTimeTracksModel(Guid memberId, List<TimeTrack> timeTrack, Member member, List<MemberProgram> memberPrograms, GenericLookup lookup)
        {
            LegacyMemberTimeTrackingsModel legacyTime = new LegacyMemberTimeTrackingsModel();
            legacyTime.TotalTime = timeTrack.Sum(m => m.TotalTime);
            legacyTime.MemberGuid = memberId;
            legacyTime.Items = ToLegacyTimeTrackingsItemModel(timeTrack, lookup, member, memberPrograms);

            return legacyTime;
        }

        public static TimeTrack FromMemberTimeTrackModelToTimeTrack(LegacyMemberTimeTrackingModel legacyTrack)
        {
            TimeTrack timeTrack = new TimeTrack
            {
                Id = legacyTrack.Id ?? Guid.NewGuid(),
                Comment = legacyTrack.AdditionalComments,
                MemberProgramId = legacyTrack.MemberProgramGuid,
                StartDate = legacyTrack.StartDate,
                EndDate = legacyTrack.EndDate,
                ServicePlanFundingSourceTypeKey = legacyTrack.FundingSource,
                Reason = legacyTrack.Reason,
                ServiceTypeKey = legacyTrack.Service,
                TimeTrackActivityTypeKey = legacyTrack.Activity,
                ServiceUnits = legacyTrack.ServiceUnits,
                MemberId = legacyTrack.MemberGuid,
                TimeTrackSubActivityTypeKey = legacyTrack.SubActivity,
                TotalTime = legacyTrack.TotalTime,
                TravelDuration = legacyTrack.TravelDuration,
                TravelMiles = legacyTrack.TravelMiles,
                VolunteerDriver = legacyTrack.VolunteerDriver ?? false,
                ProgramTypeKey = GetProgramTypeKey(legacyTrack),
                InsertedByName = legacyTrack.InsertedByName,
                InsertedOn = legacyTrack.InsertedOn,
                SegmentId = legacyTrack.SegmentId,
                UpdatedByName = legacyTrack.UpdatedByName,
                UpdatedOn = legacyTrack.UpdatedOn,
                VoidedReasonId = legacyTrack.VoidedReasonId
            };
            if (legacyTrack.Udf != null)
            {
                timeTrack.Udf = BsonDocument.Parse(legacyTrack.Udf.ToString());
            }
            return timeTrack;
        }

        private static string GetProgramTypeKey(LegacyMemberTimeTrackingModel legacyTrack)
        {
            if (!legacyTrack.MemberProgramGuid.HasValue) return legacyTrack.ProgramType;

            var programCase = legacyTrack.CaseList.FirstOrDefault(m => m.MemberProgramId == legacyTrack.MemberProgramGuid.Value);
            if (programCase != null) return programCase.ProgramTypeKey;

            return legacyTrack.ProgramType;
        }


        private static Datum GetTypeKey(List<Datum> typeTable, string key)
        {
            return typeTable.FirstOrDefault(w => w.key == key);
        }
        private static List<MemberCaseModel> GetMemberCaseModelById(IEnumerable<MemberProgram> memberPrograms, GenericLookup lookup)
        {
            List<MemberCaseModel> memberCaseModel = new List<MemberCaseModel>();
            if (memberPrograms != null && memberPrograms.Any())
            {
                foreach (var memberProgram in memberPrograms)
                {
                    MemberCaseModel memberCase = new MemberCaseModel();
                    memberCase.Case = GetCaseModel(memberProgram, lookup);
                    memberCase.MemberProgramId = memberProgram.Id;
                    memberCase.ProgramTypeKey = memberProgram.ProgramTypeKey;
                    memberCaseModel.Add(memberCase);
                }
            }
            return memberCaseModel;
        }
        private static string GetCaseModel(MemberProgram program, GenericLookup lookup)
        {
            string TypeDescription = string.Format("{0}", program.ProgramTypeKey != null ? lookup?.ProgramType.FirstOrDefault(m => m.key == program?.ProgramTypeKey).typeDescription : "")
                    + string.Format("/{0}", program.ProgramSubProgramTypeKey != null ? program.ProgramSubProgramTypeKey : "")
                    + string.Format("- {0} ({1}-{2})", program.ProgramStatusTypeKey != null ? lookup?.ProgramStatusType.FirstOrDefault(m => m.key == program?.ProgramStatusTypeKey).typeDescription : ""
                    , program.EnrollmentDate == null ? "tbd" :
                    program.EnrollmentDate.Value.ToShortDateString(), program.ClosureDate == null ? "tbd"
                     : program.ClosureDate.Value.ToShortDateString());
            return TypeDescription;
        }
        private static bool IsCareProgramRequired(List<SiteConfiguration> siteConfigurations)
        {
            var result = false;
            string configKey = "Require_CaseProgram_Selection_On_MemberObjects";
            if (siteConfigurations != null && siteConfigurations.Any())
            {
                var activeConfigItem = siteConfigurations.FirstOrDefault(x => x.activeFlag == Constants.ActiveFlag && x.configKey == configKey);
                var configValue = activeConfigItem != null ? activeConfigItem.configValue : "";
                if (configValue != string.Empty && configValue == "Yes")
                {
                    result = true;
                }
            }
            return result;
        }
        private static List<LegacyMemberTimeTrackingsItemModel> ToLegacyTimeTrackingsItemModel(List<TimeTrack> timeTracks, GenericLookup lookup, Member member, List<MemberProgram> memberPrograms)
        {
            List<LegacyMemberTimeTrackingsItemModel> LegacyTimeTrackingsModel = new List<LegacyMemberTimeTrackingsItemModel>();
            foreach (var timeTrack in timeTracks)
            {
                LegacyMemberTimeTrackingsItemModel legacyMember = new LegacyMemberTimeTrackingsItemModel();
                legacyMember.Id = timeTrack.Id;
                legacyMember.Date = timeTrack.StartDate.Date;
                legacyMember.Activity = GetTypeDescription(lookup.TimeTrackActivityType, timeTrack.TimeTrackActivityTypeKey);
                legacyMember.SubActivity = GetTypeDescription(lookup.TimeTrackSubActivityType, timeTrack.TimeTrackSubActivityTypeKey);
                legacyMember.FundingSource = GetTypeDescription(lookup.ServicePlanFundingSourceType, timeTrack.ServicePlanFundingSourceTypeKey);
                legacyMember.ServiceUnits = timeTrack.ServiceUnits;
                legacyMember.TotalTime = timeTrack.TotalTime;
                if (member != null)
                {
                    legacyMember.Member = string.Format("{0} {1}", member.FirstName, member.LastName);
                }
                legacyMember.ProgramType = GetTypeDescription(lookup.ProgramType, timeTrack.ProgramTypeKey);
                legacyMember.Service = GetTypeDescription(lookup.ServiceType, timeTrack.ServiceTypeKey);
                legacyMember.InsertedByName = timeTrack.InsertedByName;
                legacyMember.Case = GetCaseDescription(memberPrograms, timeTrack.MemberProgramId, lookup.ProgramStatusType);
                legacyMember.Version = timeTrack.Version;
                LegacyTimeTrackingsModel.Add(legacyMember);
            }
            return LegacyTimeTrackingsModel;
        }

        private static string GetCaseDescription(IEnumerable<MemberProgram> memberProgram, Guid? memberProgramId, List<Datum>lookup)
        {
            string caseDescription = "";
            if (memberProgramId == null)
            {
                return null;
            }

            if (memberProgram != null && memberProgram.Any())
            {
                var program = memberProgram.FirstOrDefault(m => m.Id == memberProgramId);
                caseDescription = program?.ProgramTypeKey;
                if (!string.IsNullOrEmpty(program.ProgramSubProgramTypeKey))
                    caseDescription += string.Format("/{0}", program.ProgramSubProgramTypeKey);

                string closureDate = program.ClosureDate == null ? "tbd" : program.ClosureDate.Value.ToShortDateString();
                string enrollmentDate = program.EnrollmentDate == null ? "tbd" : program.EnrollmentDate.Value.ToShortDateString();
                var statusType = lookup.FirstOrDefault(w => w.key == program.ProgramStatusTypeKey);
                caseDescription += string.Format(" - {0} ({1}-{2})", statusType?.typeDescription, enrollmentDate, closureDate);
            }
            return caseDescription;
        }
        private static string GetTypeDescription(IEnumerable<Datum> datum, string key)
        {
            var result = datum.FirstOrDefault(m => m.key == key);
            if (result == null)
            {
                return key;
            }
            return result.typeDescription;
        }

    }
}
