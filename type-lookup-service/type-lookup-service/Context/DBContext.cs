using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Newtonsoft.Json;
using type_lookup_service.Model;
using type_lookup_service.Models;
using type_lookup_service.Utils;

namespace type_lookup_service.Context
{
    public class DbContext : IDbContext
    {
        private readonly ILogger _logger;
        private readonly MongoClientConnectionSettings _settings;

        public IMongoDatabase Database { get; private set; }
        public IMongoCollection<Authorization> AuthorizationTableCollection => Database.GetCollection<Authorization>(_settings.AuthorizationTableCollection);
        public IMongoCollection<AssessmentTemplate> AssessmentTemplateCollection => Database.GetCollection<AssessmentTemplate>(_settings.AssessmentTemplateCollection);
        public IMongoCollection<AssessmentPage> AssessmentPageCollection => Database.GetCollection<AssessmentPage>(_settings.AssessmentPageCollection);
        public IMongoCollection<AssessmentTab> AssessmentTabCollection => Database.GetCollection<AssessmentTab>(_settings.AssessmentTabCollection);
        public IMongoCollection<AssessmentSection> AssessmentSectionCollection => Database.GetCollection<AssessmentSection>(_settings.AssessmentSectionCollection);
        public IMongoCollection<AssessmentQuestion> AssessmentQuestionCollection => Database.GetCollection<AssessmentQuestion>(_settings.AssessmentQuestionCollection);
        public IMongoCollection<AssessmentAnswer> AssessmentAnswerCollection => Database.GetCollection<AssessmentAnswer>(_settings.AssessmentAnswerCollection);
        public IMongoCollection<Medication> MedicationCollection => Database.GetCollection<Medication>(_settings.MedicationCollection);
        public IMongoCollection<MedicationProductDescription> MedicationProductDescriptionCollection => Database.GetCollection<MedicationProductDescription>(_settings.MedicationProductDescriptionCollection);
        public IMongoCollection<MedicationManufacturer> MedicationManufacturerCollection => Database.GetCollection<MedicationManufacturer>(_settings.MedicationManufacturerCollection);
        public IMongoCollection<MedicationPackageDescription> MedicationPackageDescriptionCollection => Database.GetCollection<MedicationPackageDescription>(_settings.MedicationPackageDescriptionCollection);

        public DbContext(
            IContextLogger<DbContext> logger,
            MongoClientConnectionSettings settings)
        {
            _logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
            _settings = settings ?? throw new System.ArgumentNullException(nameof(settings));
        }

        public void Initialize()
        {
            var dbClient = new MongoClient(_settings.GenerateSettings());
            Database = dbClient.GetDatabase(_settings.Database);

            var databaseList = dbClient.ListDatabaseNames().ToList();
            var databaseExists = databaseList.Contains(_settings.Database);

            if (databaseExists)
            {
                _logger.LogInformation($"Database '${_settings.Database}' exists");
                var collectionInfoString = JsonConvert.SerializeObject(new
                {
                    _settings.ConfigCollection,
                    _settings.AssessmentAnswerCollection,
                    _settings.AssessmentPageCollection,
                    _settings.AssessmentQuestionCollection,
                    _settings.AssessmentSectionCollection,
                    _settings.AssessmentTabCollection,
                    _settings.AssessmentTemplateCollection,
                    _settings.AuthorizationTableCollection,
                    _settings.MedicationCollection,
                    _settings.MedicationProductDescriptionCollection,
                    _settings.MedicationManufacturerCollection,
                    _settings.MedicationPackageDescriptionCollection
                });
                _logger.LogInformation($"Collection settings: {collectionInfoString}");
            }
            else
            {
                _logger.LogInformation($"Database '${_settings.Database}' does not exist.  Creating collections...");
                Database.CreateCollection(_settings.ConfigCollection);
                _logger.LogInformation($"{_settings.ConfigCollection} '${_settings.ConfigCollection}' created");
                Database.CreateCollection(_settings.AuthorizationTableCollection);
                _logger.LogInformation($"{_settings.AuthorizationTableCollection} '${_settings.AuthorizationTableCollection}' created");
                Database.CreateCollection(_settings.AssessmentTemplateCollection);
                _logger.LogInformation($"{_settings.AssessmentTemplateCollection} '${_settings.AssessmentTemplateCollection}' created");
                Database.CreateCollection(_settings.AssessmentPageCollection);
                _logger.LogInformation($"{_settings.AssessmentPageCollection} '${_settings.AssessmentPageCollection}' created");
                Database.CreateCollection(_settings.AssessmentTabCollection);
                _logger.LogInformation($"{_settings.AssessmentTabCollection} '${_settings.AssessmentTabCollection}' created");
                Database.CreateCollection(_settings.AssessmentSectionCollection);
                _logger.LogInformation($"{_settings.AssessmentSectionCollection} '${_settings.AssessmentSectionCollection}' created");
                Database.CreateCollection(_settings.AssessmentQuestionCollection);
                _logger.LogInformation($"{_settings.AssessmentQuestionCollection} '${_settings.AssessmentQuestionCollection}' created");
                Database.CreateCollection(_settings.AssessmentAnswerCollection);
                _logger.LogInformation($"{_settings.AssessmentAnswerCollection} '${_settings.AssessmentAnswerCollection}' created");
                Database.CreateCollection(_settings.MedicationCollection);
                _logger.LogInformation($"{_settings.MedicationCollection} '${_settings.MedicationCollection}' created");
                Database.CreateCollection(_settings.MedicationProductDescriptionCollection);
                _logger.LogInformation($"{_settings.MedicationProductDescriptionCollection} '${_settings.MedicationProductDescriptionCollection}' created");
                Database.CreateCollection(_settings.MedicationManufacturerCollection);
                _logger.LogInformation($"{_settings.MedicationManufacturerCollection} '${_settings.MedicationManufacturerCollection}' created");
                Database.CreateCollection(_settings.MedicationPackageDescriptionCollection);
                _logger.LogInformation($"{_settings.MedicationPackageDescriptionCollection} '${_settings.MedicationPackageDescriptionCollection}' created");
            }
        }
    }
}
