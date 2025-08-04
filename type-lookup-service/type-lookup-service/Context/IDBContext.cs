using MongoDB.Driver;
using type_lookup_service.Model;
using type_lookup_service.Models;

namespace type_lookup_service.Context
{
    public interface IDbContext
    {
        IMongoDatabase Database { get; }
        IMongoCollection<Authorization> AuthorizationTableCollection { get; }
        IMongoCollection<AssessmentTemplate> AssessmentTemplateCollection { get; }
        IMongoCollection<AssessmentPage> AssessmentPageCollection { get; }
        IMongoCollection<AssessmentTab> AssessmentTabCollection { get; }
        IMongoCollection<AssessmentSection> AssessmentSectionCollection { get; }
        IMongoCollection<AssessmentQuestion> AssessmentQuestionCollection { get; }
        IMongoCollection<AssessmentAnswer> AssessmentAnswerCollection { get; }
        IMongoCollection<Medication> MedicationCollection { get; }
        IMongoCollection<MedicationProductDescription> MedicationProductDescriptionCollection { get; }
        IMongoCollection<MedicationManufacturer> MedicationManufacturerCollection { get; }
        IMongoCollection<MedicationPackageDescription> MedicationPackageDescriptionCollection { get; }

        void Initialize();
    }
}