using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using type_lookup_service.Model;
using type_lookup_service.Models;

namespace type_lookup_service.Data
{
    public interface IRepository
    {
        Task<List<AssessmentAnswer>> GetAssessmentAnswer(Guid[] assessmentQuestionIds);
        Task<List<AssessmentPage>> GetAssessmentPages(Guid assessmentTemplateId);
        Task<List<AssessmentQuestion>> GetAssessmentQuestions(Guid[] assessmentSectionIds);
        Task<List<AssessmentSection>> GetAssessmentSections(Guid[] assessmentTabIds);
        Task<List<AssessmentTab>> GetAssessmentTabs(Guid[] assessmentPageIds);
        Task<AssessmentTemplate> GetAssessmentTemplate(Guid assessmentTemplateId);
        Task<List<AssessmentTemplate>> GetAssessmentTemplates();
        Task<IEnumerable<Authorization>> GetAuthorizationsAsync(IEnumerable<Guid> securityRoleIds, IEnumerable<string> objects, CancellationToken cancellationToken = default);
        Task<List<object>> GetLookupData(SearchModel searchModel);
        Task<(DbResponse response, List<Medication> data)> GetMedications(List<Guid> medicationIds);
        Task<(DbResponse response, Medication data)> GetMedication(Guid medicationId);
        Task<(DbResponse response, List<MedicationProductDescription> data)> GetProductDescriptionsByIdsAsync(List<Guid> productDescriptionIds, bool includeInactive = false);
        Task<(DbResponse response, List<MedicationManufacturer> data)> GetManufacturerByIdsAsync(List<Guid> manufacturerIds, bool includeInactive = false);
        Task<(DbResponse response, List<MedicationPackageDescription> data)> GetPackageDescriptionIdsAsync(List<Guid> packageDescriptionIds, bool includeInactive = false);
    }
}
