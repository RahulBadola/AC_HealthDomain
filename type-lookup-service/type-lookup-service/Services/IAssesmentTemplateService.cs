using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using type_lookup_service.Model;

namespace type_lookup_service.Services
{
    public interface IAssessmentTemplateService
    {
        public Task<AssessmentTemplate> GetAssessmentTemplateById(Guid assessmentTemplateId);
        public Task<List<AssessmentTemplate>> GetAssessmentTemplates();
    }
}
