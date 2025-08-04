using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace type_lookup_service.Model
{
    public class AssessmentSection
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public Guid AssessmentSectionId { get; set; }

        public Guid AssessmentTabId { get; set; }

        public int? Sequence { get; set; }

        public string SectionName { get; set; }

        public string SectionDescription { get; set; }

        public string TitleHeaderText { get; set; }

        public string Instructions { get; set; }

        public string AssessmentInitialStateTypeKey { get; set; }

        public string HelpText { get; set; }

        public bool RepeatedInd { get; set; }

        public bool DataMappingEnabled { get; set; }

        public string ValuePathString { get; set; }

        public int? Xcoordinate { get; set; }

        public int? Ycoordinate { get; set; }

        public int? Height { get; set; }

        public int? Width { get; set; }

        public string Score { get; set; }

        public string AssessmentControlTypeKey { get; set; }

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


        [BsonIgnore]
        public IEnumerable<AssessmentQuestion> Questions { get; set; }
    }

}
