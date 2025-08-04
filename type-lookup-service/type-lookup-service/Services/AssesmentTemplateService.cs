using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using type_lookup_service.Data;
using type_lookup_service.Model;
using type_lookup_service.Utils;

namespace type_lookup_service.Services
{
    public class AssessmentTemplateService : IAssessmentTemplateService
    {
        private readonly ILogger _logger;
        private readonly IRepository _repository;

        public AssessmentTemplateService(
            IContextLogger<AssessmentTemplateService> logger,
            IRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        public async Task<AssessmentTemplate> GetAssessmentTemplateById(Guid assessmentTemplateId)
        {
            var assessmentTemplate = new AssessmentTemplate();
            try
            {
                assessmentTemplate = await _repository.GetAssessmentTemplate(assessmentTemplateId);

                if (assessmentTemplate != null && !assessmentTemplate.HasError)
                {
                    assessmentTemplate.Pages = await _repository.GetAssessmentPages(assessmentTemplateId);
                    if (assessmentTemplate.Pages != null && assessmentTemplate.Pages.Any())
                    {
                        var pageIds = assessmentTemplate.Pages.Select(x => x.AssessmentPageId).ToArray();

                        var tabs = await _repository.GetAssessmentTabs(pageIds);

                        if (tabs != null && tabs.Any())
                        {
                            foreach (var page in assessmentTemplate.Pages)
                            {
                                page.Tabs = tabs.Where(t => t.AssessmentPageId == page.AssessmentPageId);
                            }

                            var tabIds = tabs.Select(x => x.AssessmentTabId).ToArray();

                            var sections = await _repository.GetAssessmentSections(tabIds);

                            if (sections != null && sections.Any())
                            {
                                foreach (var page in assessmentTemplate.Pages)
                                {
                                    foreach (var tab in page.Tabs)
                                    {
                                        tab.Sections = sections.Where(p => p.AssessmentTabId == tab.AssessmentTabId);
                                    }
                                }

                                var sectionIds = sections.Select(x => x.AssessmentSectionId).ToArray();
                                var questions = await _repository.GetAssessmentQuestions(sectionIds);

                                if (questions != null && questions.Any())
                                {
                                    foreach (var page in assessmentTemplate.Pages)
                                    {
                                        foreach (var tab in page.Tabs)
                                        {
                                            foreach (var section in tab.Sections)
                                            {
                                                section.Questions = questions.Where(p => p.AssessmentSectionId == section.AssessmentSectionId);
                                            }
                                        }
                                    }

                                    var questionIds = questions.Select(x => x.AssessmentQuestionId).ToArray();
                                    var answers = await _repository.GetAssessmentAnswer(questionIds);

                                    if (answers != null && answers.Any())
                                    {
                                        foreach (var page in assessmentTemplate.Pages)
                                        {
                                            foreach (var tab in page.Tabs)
                                            {
                                                foreach (var section in tab.Sections)
                                                {
                                                    foreach (var question in section.Questions)
                                                    {
                                                        question.Answers = answers.Where(a => a.AssessmentQuestionId == question.AssessmentQuestionId);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occured when gettingAssessment Data {ex}");
                if (assessmentTemplate == null)
                    assessmentTemplate = new AssessmentTemplate();

                assessmentTemplate.HasError = true;
                assessmentTemplate.ErrorMessage = ex.Message + ex.StackTrace;
                assessmentTemplate.AssessmentTemplateId = assessmentTemplateId;
            }

            return assessmentTemplate;
        }

        public async Task<List<AssessmentTemplate>> GetAssessmentTemplates()
        {
            try
            {
                return await _repository.GetAssessmentTemplates();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occured when gettingAssessment Data {ex}");
            }

            return new List<AssessmentTemplate>();
        }
    }
}
