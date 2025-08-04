using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace type_lookup_service.Model
{
    [BsonIgnoreExtraElements]
    public class AssessmentTemplate
    {
        public AssessmentTemplate()
        {
            Pages = new List<AssessmentPage>();
        }
        [BsonId]
        public ObjectId Id { get; set; }

        public Guid AssessmentTemplateId { get; set; }

        public string TemplateName { get; set; }

        public string TemplateDescription { get; set; }

        public double Version { get; set; }

        public string TitleHeaderText { get; set; }

        public string Instructions { get; set; }

        public string HelpText { get; set; }

        public bool DataMappingEnabled { get; set; }

        public string Score { get; set; }

        public string Stratification { get; set; }

        public string SaveMessage { get; set; }

        public string CompletedMessage { get; set; }

        public string AssessmentTemplateTypeKey { get; set; }

        public string ObjectTypeKey { get; set; }

        public bool PasEnabledFlag { get; set; }

        public bool ProviderPortalFlag { get; set; }

        public string EffectiveDate { get; set; }

        public string ExpirationDate { get; set; }

        public bool ESignatureFlag { get; set; }

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

        public int ActiveFlag { get; set; }

        public Guid SegmentId { get; set; }

        public bool IncludeProgressMeter { get; set; }

        public bool AdvancedNavigation { get; set; }

        public bool CollapseSection { get; set; }

        public bool EnableProgramTypeAssociation { get; set; }
        [BsonIgnore]
        public IEnumerable<AssessmentPage> Pages { get; set; }

        public bool HasError { get; set; }

        public string ErrorMessage { get; set; }
    }


}
