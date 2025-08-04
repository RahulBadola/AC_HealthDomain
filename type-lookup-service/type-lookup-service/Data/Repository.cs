using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using type_lookup_service.Context;
using type_lookup_service.Model;
using type_lookup_service.Models;
using type_lookup_service.Utils;

namespace type_lookup_service.Data
{
    internal class Repository : IRepository
    {
        public Repository(
            IDbContext dbContext,
            ILogger<Repository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<List<AssessmentAnswer>> GetAssessmentAnswer(Guid[] assessmentQuestionIds)
        {
            try
            {
                var builder = Builders<AssessmentAnswer>.Filter;
                var filter = builder.In(b => b.AssessmentQuestionId, assessmentQuestionIds);

                var queryableResult = await _dbContext.AssessmentAnswerCollection.Find(filter).ToListAsync();

                return queryableResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }

            return new List<AssessmentAnswer>();
        }

        public async Task<List<AssessmentSection>> GetAssessmentSections(Guid[] assessmentTabIds)
        {
            try
            {
                var builder = Builders<AssessmentSection>.Filter;
                var filter = builder.In(b => b.AssessmentTabId, assessmentTabIds);

                var queryableResult = await _dbContext.AssessmentSectionCollection.Find(filter).ToListAsync();

                return queryableResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }

            return new List<AssessmentSection>();
        }

        public async Task<List<AssessmentPage>> GetAssessmentPages(Guid assessmentTemplateId)
        {
            var assessmentPages = new List<AssessmentPage>();
            try
            {
                var result = _dbContext.AssessmentPageCollection.AsQueryable().Where(page => page.AssessmentTemplateId == assessmentTemplateId);

                assessmentPages = await result.ToListAsync();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }

            return assessmentPages;
        }

        public async Task<List<AssessmentQuestion>> GetAssessmentQuestions(Guid[] assessmentSectionIds)
        {
            try
            {
                var builder = Builders<AssessmentQuestion>.Filter;
                var filter = builder.In(b => b.AssessmentSectionId, assessmentSectionIds);

                var queryableResult = await _dbContext.AssessmentQuestionCollection.Find(filter).ToListAsync();

                return queryableResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }

            return new List<AssessmentQuestion>();
        }

        public async Task<List<AssessmentTab>> GetAssessmentTabs(Guid[] assessmentPageIds)
        {
            try
            {
                var builder = Builders<AssessmentTab>.Filter;
                var filter = builder.In(b => b.AssessmentPageId, assessmentPageIds);

                var queryableResult = await _dbContext.AssessmentTabCollection.Find(filter).ToListAsync();

                return queryableResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }

            return new List<AssessmentTab>();
        }

        public async Task<AssessmentTemplate> GetAssessmentTemplate(Guid assessmentTemplateId)
        {
            var assessmentTemplate = new AssessmentTemplate();
            try
            {
                var result = _dbContext.AssessmentTemplateCollection.AsQueryable().Where(template => template.AssessmentTemplateId == assessmentTemplateId);

                assessmentTemplate = await result.FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                assessmentTemplate.ErrorMessage = ex.Message + ex.StackTrace;
                assessmentTemplate.AssessmentTemplateId = assessmentTemplateId;
                assessmentTemplate.HasError = true;

                _logger.LogError(ex, ex.Message);
            }

            return assessmentTemplate;
        }

        public async Task<List<AssessmentTemplate>> GetAssessmentTemplates()
        {
            var assessmentTemplate = new List<AssessmentTemplate>();
            try
            {
                assessmentTemplate = await _dbContext.AssessmentTemplateCollection.AsQueryable().ToListAsync();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }

            return assessmentTemplate;
        }

        public async Task<IEnumerable<Authorization>> GetAuthorizationsAsync(IEnumerable<Guid> securityRoleIds, IEnumerable<string> objects, CancellationToken cancellationToken = default)
            => await _dbContext.AuthorizationTableCollection
                .Find(BuildAuthorizationsFilter(securityRoleIds, objects))
                .ToListAsync(cancellationToken);

        public async Task<List<object>> GetLookupData(SearchModel searchModel)
        {
            try
            {
                var filter = Builders<BsonDocument>.Filter.Regex(searchModel.PrimaryKeyName, new BsonRegularExpression(Regex.Escape(searchModel.PrimayKeyValue), "i"));

                var collection = _dbContext.Database.GetCollection<BsonDocument>(searchModel.ObjectName);

                var data = await (await collection.FindAsync<BsonDocument>(filter).ConfigureAwait(false)).ToListAsync();

                return data.ConvertAll(BsonTypeMapper.MapToDotNetValue);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }

            return new List<Object>();
        }
        public async Task<(DbResponse response, List<Medication> data)> GetMedications(List<Guid> medicationIds)
        {
            if (medicationIds.Count > 200)
            {
                _logger.LogError("Querying more than 200 medication ids is forbiddem");
                return (DbResponse.Error, null);
            }

            try
            {
                var builder = Builders<Medication>.Filter;
                var filter = builder.In(b => b.Id, medicationIds);

                var queryableResult = await _dbContext.MedicationCollection.Find(filter).ToListAsync();

                if(queryableResult == null || !queryableResult.Any())
                {
                    return (DbResponse.NotFound, null);
                }
                return (DbResponse.Found, queryableResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return (DbResponse.Error, null);
            }
        }

        public async Task<(DbResponse response, Medication data)> GetMedication(Guid medicationId)
        {
            try
            {
                var result = _dbContext.MedicationCollection.AsQueryable().Where(mc => mc.Id == medicationId);

                if (result == null || !result.Any())
                {
                    return (DbResponse.NotFound, null);
                }

                var medication = await result.FirstOrDefaultAsync();

                return (DbResponse.Found, medication);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return (DbResponse.Error, null);
            }
        }
        public async Task<(DbResponse response, List<MedicationProductDescription> data)> GetProductDescriptionsByIdsAsync(List<Guid> productDescriptionIds, bool includeInactive=false)
        {
            try
            {
                var builder = Builders<MedicationProductDescription>.Filter;
                var filter = builder.In(b => b.Id, productDescriptionIds);
                if (!includeInactive)
                {
                    filter &= Common.GetActiveFilterDefinition<MedicationProductDescription>();
                }
                var result = await _dbContext.MedicationProductDescriptionCollection.Find(filter).ToListAsync();
                if (result == null || !result.Any())
                {
                    return (DbResponse.NotFound, null);
                }
                return (DbResponse.Found, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return (DbResponse.Error, null);
            }
        }
        public async Task<(DbResponse response, List<MedicationManufacturer> data)> GetManufacturerByIdsAsync(List<Guid> manufacturerIds, bool includeInactive = false)
        {
            try
            {
                var builder = Builders<MedicationManufacturer>.Filter;
                var filter = builder.In(b => b.Id, manufacturerIds);
                if (!includeInactive)
                {
                    filter &= Common.GetActiveFilterDefinition<MedicationManufacturer>();
                }
                var result = await _dbContext.MedicationManufacturerCollection.Find(filter).ToListAsync();
                if (result == null || !result.Any())
                {
                    return (DbResponse.NotFound, null);
                }
                return (DbResponse.Found, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return (DbResponse.Error, null);
            }
        }
        public async Task<(DbResponse response, List<MedicationPackageDescription> data)> GetPackageDescriptionIdsAsync(List<Guid> packageDescriptionIds, bool includeInactive = false)
        {
            try
            {
                var builder = Builders<MedicationPackageDescription>.Filter;
                var filter = builder.In(b => b.Id, packageDescriptionIds);
                if (!includeInactive)
                {
                    filter &= Common.GetActiveFilterDefinition<MedicationPackageDescription>();
                }
                var result = await _dbContext.MedicationPackageDescriptionCollection.Find(filter).ToListAsync();
                if (result == null || !result.Any())
                {
                    return (DbResponse.NotFound, null);
                }
                return (DbResponse.Found, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return (DbResponse.Error, null);
            }
        }
        #region internal

        internal static FilterDefinition<Authorization> BuildAuthorizationsFilter(IEnumerable<Guid> securityRoleIds, IEnumerable<string> objects)
        {
            var builder = Builders<Authorization>.Filter;
            return builder.In(b => b.ObjectSid, securityRoleIds.Select(g => g.ToString()))
                & builder.In(b => b.Object, objects)
                & builder.In(b => b.AuthorizationType, DefaultAuthorizationTypes.Cast<int?>());
        }

        #endregion

        #region private

        private readonly IDbContext _dbContext;
        private readonly ILogger<Repository> _logger;

        private static readonly int[] DefaultAuthorizationTypes = new[] { 1, 3 };

        #endregion
    }
}
