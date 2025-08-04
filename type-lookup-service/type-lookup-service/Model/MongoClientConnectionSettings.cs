using MongoDB.Driver;
using System;
using System.Security.Cryptography.X509Certificates;

namespace type_lookup_service.Model
{
    public class MongoClientConnectionSettings
    {
        public string ConnectionUrl { get; set; }
        public string ConfigCollection { get; set; } = nameof(ConfigCollection);
        public string AuthorizationTableCollection { get; set; } = "AuthorizationsTable";
        public string AssessmentTemplateCollection { get; set; } = "AssessmentTemplate";
        public string AssessmentPageCollection { get; set; } = "AssessmentPage";
        public string AssessmentTabCollection { get; set; } = "AssessmentTab";
        public string AssessmentSectionCollection { get; set; } = "AssessmentSection";
        public string AssessmentQuestionCollection { get; set; } = "AssessmentQuestion";
        public string AssessmentAnswerCollection { get; set; } = "AssessmentAnswer";
        public string MedicationCollection { get; set; } = "Medication";
        public string MedicationProductDescriptionCollection { get; set; }
        public string MedicationManufacturerCollection { get; set; }
        public string MedicationPackageDescriptionCollection { get; set; }
        public string Database { get; set; }
        public string PublicKeyPassword { get; set; }
        public string ClientAccessPFXPath { get; set; }
        public bool UseTls { get; set; }
        public bool UsePublicKeyConnection { get; set; }

        public MongoClientSettings GenerateSettings()
        {
            var url = Environment.ExpandEnvironmentVariables(ConnectionUrl);

            var mongoDbClientSettings = MongoClientSettings.FromUrl(MongoUrl.Create(url));
            if (UseTls)
            {
                mongoDbClientSettings.UseTls = true;
                mongoDbClientSettings.AllowInsecureTls = true;

                if (UsePublicKeyConnection)
                {
                    mongoDbClientSettings.AllowInsecureTls = false;

                    var certificate =
                        new X509Certificate2(ClientAccessPFXPath, PublicKeyPassword);
                    var certs = new[] { certificate };
                    var sslSettings = new SslSettings
                    {
                        ClientCertificates = certs,
                        ClientCertificateSelectionCallback = delegate
                        {
                            return certs[0];
                        },
                        ServerCertificateValidationCallback = delegate
                        {
                            return true;
                        }
                    };
                    mongoDbClientSettings.SslSettings = sslSettings;
                }
            }

            return mongoDbClientSettings;

        }
        public override string ToString()
        {
            return $"Server DB Connection URL: {ConnectionUrl}, " +
                $"Collection: {ConfigCollection}, " +
                $"UseTls: {UseTls}, " +
                $"UsePublicKeyConnection: {UsePublicKeyConnection}";
        }
    }
}