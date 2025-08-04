using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace type_lookup_service.Model
{
    public class AssessmentAnswer
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public Guid AssessmentAnswerId { get; set; }

        public string AssessmentAnswerTypeKey { get; set; }

        public Guid AssessmentQuestionId { get; set; }

        public Guid AssessmentListId { get; set; }

        public int? Sequence { get; set; }

        public bool HorizontalAlignment { get; set; }

        public string AnswerText { get; set; }

        public string DefaultValue { get; set; }

        public string ReqexValidator { get; set; }

        public string DateRangeMin { get; set; }

        public string DateRangeMax { get; set; }

        public string NumRangeMin { get; set; }

        public string NumRangeMax { get; set; }

        public int? NumPrecision { get; set; }

        public int? MaxLength { get; set; }

        public string WeightedValue { get; set; }

        public int? DaysOfPrevData { get; set; }

        public int? NumOfPrevData { get; set; }

        public string MatrixRowChoices { get; set; }

        public string MatrixColumnChoices { get; set; }

        public bool DisplayHorizontalFlag { get; set; }

        public bool RequiredInd { get; set; }

        public string AssessmentInitialStateTypeKey { get; set; }

        public bool SummaryInd { get; set; }

        public string SummaryTitle { get; set; }

        public bool IncludeInScoring { get; set; }

        public bool AlphaSorting { get; set; }

        public int? Xcoordinate { get; set; }

        public int? Ycoordinate { get; set; }

        public int? Height { get; set; }

        public int? Width { get; set; }

        public int? TabOrder { get; set; }

        public bool CopyAnswer { get; set; }

        public Guid AssessmentAnswerMergeFieldMapId { get; set; }

        public Guid MergeAssessmentAnswerId { get; set; }

        public string AssessmentAnswerNumber { get; set; }

        public bool AnswerNumberDisplayFlag { get; set; }

        public string AssessmentAnswerMergeTypeKey { get; set; }

        public string EffectiveDate { get; set; }

        public string ExpirationDate { get; set; }

        public Guid InsertedBy { get; set; }

        public Guid InsertedById { get; set; }

        public string InsertedOn { get; set; }

        public Guid UpdatedBy { get; set; }

        public Guid UpdatedById { get; set; }

        public string UpdatedOn { get; set; }

        public Guid? VoidedBy { get; set; }

        public Guid? VoidedById { get; set; }

        public string VoidedOn { get; set; }

        public Guid? VoidedReasonId { get; set; }

        public string ConversionId { get; set; }

        public int? ActiveFlag { get; set; }

        public Guid SegmentId { get; set; }

        public string AnswerDynamicDictionaryRootObjectTypeKey { get; set; }

        public string AnswerDynamicDictionaryObjectTypeKey { get; set; }

        public string AnswerDynamicDictionaryMedReviewObjectTypeKey { get; set; }

        public string AnswerDynamicDictionaryDropdownTypeKey { get; set; }

        public string AnswerDynamicDictionaryOptionDescriptionTypeKey { get; set; }

        public string AnswerDynamicDictionaryOptionValueTypeKey { get; set; }

        public bool AnswerDynamicDictionaryPullOnlyActiveFlag { get; set; }

        public bool DisplayAnswerNameFlag { get; set; }

        public string AnswerListDefaultValue { get; set; }

        public string MutuallyExclusive { get; set; }

        public bool PullFromCurrentAssessment { get; set; }

    }
}
