using System;
using MongoDB.Driver;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace time_track_service.Model.Dto
{
    public class MongoClientConnectionSettings
    {
        public string ConnectionUrl { get; set; }
        public string Database { get; set; }
        public string TimeTrackCollection { get; set; }


        public string TimeTrackConfigurationDetailCollection { get; set; }

        public string PublicKeyPassword { get; set; }
        public string ClientAccessPFXPath { get; set; }
        public bool UseTls { get; set; }
        public bool UsePublicKeyConnection { get; set; }

        public MongoDB.Driver.MongoClientSettings GenerateSettings()
        {
            var url = Environment.ExpandEnvironmentVariables(this.ConnectionUrl);

            var mongoDBClientSettings = MongoDB.Driver.MongoClientSettings.FromUrl(MongoUrl.Create(url));
            if (this.UseTls)
            {
                mongoDBClientSettings.UseTls = true;
                mongoDBClientSettings.AllowInsecureTls = true;

                if (this.UsePublicKeyConnection)
                {
                    mongoDBClientSettings.AllowInsecureTls = false;

                    X509Certificate2 certificate =
                        new X509Certificate2(this.ClientAccessPFXPath, this.PublicKeyPassword);
                    var certs = new[] {certificate};
                    var sslSettings = new SslSettings
                    {
                        ClientCertificates = certs,
                        ClientCertificateSelectionCallback = delegate(object sender, string targetHost,
                            X509CertificateCollection localCertificates, X509Certificate remoteCertificate,
                            string[] acceptableIssuers)
                        {
                            return certs[0];
                        },
                        ServerCertificateValidationCallback = delegate(object sender, X509Certificate cert,
                            X509Chain chain, SslPolicyErrors sslPolicyErrors)
                        {
                            return true;
                        }
                    };
                    mongoDBClientSettings.SslSettings = sslSettings;
                }
            }

            return mongoDBClientSettings;

        }
        public override string ToString()
        {
            return $"Server DB Connection URL: {ConnectionUrl}, " + 
                $"Database: {Database}, " +
                $"UseTls: {UseTls}, " +
                $"UsePublicKeyConnection: {UsePublicKeyConnection}";
        }
    }
}
