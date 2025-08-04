using Confluent.SchemaRegistry;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace time_track_dshp.Utils.Serialization.Confluent
{
    public sealed class LocalSchemaRegistryClient : ISchemaRegistryClient
    {
        private readonly string _schema;

        public LocalSchemaRegistryClient(string schema)
        {
            _schema = schema;
        }

        public LocalSchemaRegistryClient(Uri uri)
        {
            _schema = File.ReadAllText(uri.LocalPath, Encoding.UTF8);
        }

        public int MaxCachedSchemas => 100;

        public string ConstructKeySubjectName(string topic, string recordType = null)
        {
            return topic;
        }

        public string ConstructValueSubjectName(string topic, string recordType = null)
        {
            return topic;
        }

        public void Dispose()
        {
            //Ignoring. There's nothing unmanaged happening here.
        }

        public Task<List<string>> GetAllSubjectsAsync()
        {
            return Task.FromResult(new List<string> { _schema });
        }

        public Task<RegisteredSchema> GetRegisteredSchemaAsync(string subject, int version)
        {
            return Task.FromResult(new RegisteredSchema(subject, version, 1, _schema, SchemaType.Avro, null));
        }

        public Task<string> GetSchemaAsync(int id)
        {
            return Task.FromResult(_schema);
        }

        public Task<string> GetSchemaAsync(string subject, int version)
        {
            return Task.FromResult(_schema);
        }

        public Task<Schema> GetSchemaAsync(int id, string format = null)
        {
            return Task.FromResult(new Schema(_schema, SchemaType.Avro));
        }

        public Task<int> GetSchemaIdAsync(string subject, Schema schema)
        {
            return Task.FromResult(1);
        }

        public Task<int> GetSchemaIdAsync(string subject, string avroSchema)
        {
            return Task.FromResult(1);
        }

        public Task<List<int>> GetSubjectVersionsAsync(string subject)
        {
            return Task.FromResult(new List<int> { 1 });
        }

        public Task<bool> IsCompatibleAsync(string subject, string avroSchema)
        {
            return Task.FromResult(true);
        }

        public Task<bool> IsCompatibleAsync(string subject, Schema schema)
        {
            return Task.FromResult(true);
        }

        public Task<RegisteredSchema> LookupSchemaAsync(string subject, Schema schema, bool ignoreDeletedSchemas)
        {
            return Task.FromResult(new RegisteredSchema(subject, 1, 1, _schema, SchemaType.Avro, null));
        }

        public Task<int> RegisterSchemaAsync(string subject, string avroSchema)
        {
            return Task.FromResult(1);
        }

        public Task<int> RegisterSchemaAsync(string subject, Schema schema)
        {
            return Task.FromResult(1);
        }

        Task<RegisteredSchema> ISchemaRegistryClient.GetLatestSchemaAsync(string subject)
        {
            return Task.FromResult(new RegisteredSchema(subject, 1, 1, _schema, SchemaType.Avro, null));
        }
    }
}