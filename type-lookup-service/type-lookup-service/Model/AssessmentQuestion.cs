using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace type_lookup_service.Model
{
    public class AssessmentQuestion
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public Guid AssessmentQuestionId { get; set; }

        public Guid AssessmentSectionId { get; set; }

        public int? Sequence { get; set; }

        public string QuestionText { get; set; }

        public string AltQuestionText { get; set; }

        public string TitleHeaderText { get; set; }

        public string Instructions { get; set; }

        public string HelpText { get; set; }

        public string AssessmentInitialStateTypeKey { get; set; }

        public int? Xcoordinate { get; set; }

        public int? Ycoordinate { get; set; }

        public int? Height { get; set; }

        public int? Width { get; set; }

        public string AssessmentQuestionNumber { get; set; }

        public bool QuestionNumberDisplayFlag { get; set; }

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

        public bool HorizontalAlignment { get; set; }

        public bool DisplayAfterRepeatableSection { get; set; }

        [BsonIgnore]
        public IEnumerable<AssessmentAnswer> Answers { get; set; }

    }

}
