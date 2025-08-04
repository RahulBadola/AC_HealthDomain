using Avro;
using Avro.Generic;
using Confluent.Kafka;
using Confluent.Kafka.SyncOverAsync;
using Confluent.SchemaRegistry.Serdes;
using time_track_dshp.Extensions;
using time_track_dshp.Models.Configuration;
using time_track_dshp.Models.Dto;
using time_track_dshp.Utils.Serialization.Confluent;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace time_track_dshp.Services.Hosted
{
    public class SyncBackMessageConsumerService : BackgroundService
    {
        private readonly ILogger<SyncBackMessageConsumerService> _logger;
        private readonly MessagingConfiguration _messagingConfiguration;
        private ConsumerConfig _consumerConfig;
        private readonly ISyncBackService _syncBackService;
        private readonly RecordSchema _schema;

        public SyncBackMessageConsumerService(ILogger<SyncBackMessageConsumerService> logger,
            MessagingConfiguration messagingConfiguration,
            ISyncBackService syncBackService)
        {
            _logger = logger;
            _messagingConfiguration = messagingConfiguration;
            _syncBackService = syncBackService;
            _schema = (RecordSchema)Schema.Parse(File.ReadAllText(_messagingConfiguration.Kafka.SyncBackConsumerSchemaFileLocation));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Initialize();
            try
            {
                await Task.Factory.StartNew(() => ConsumeMessage(stoppingToken), TaskCreationOptions.LongRunning);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw;
            }
        }

        public void ConsumeMessage(CancellationToken stoppingToken)
        {
            var schema = File.ReadAllText(_messagingConfiguration.Kafka.SyncBackConsumerSchemaFileLocation);

            using (var consumer = new ConsumerBuilder<Null, GenericRecord>(_consumerConfig)
                .SetValueDeserializer(new AvroDeserializer<GenericRecord>(new LocalSchemaRegistryClient(schema)).AsSyncOverAsync())
                .Build())
            {
                consumer.Subscribe(_messagingConfiguration.Kafka.SyncBackConsumerTopic);

                try
                {
                    while (true)
                    {
                        try
                        {
                            var consumeResult = consumer.Consume(stoppingToken);
                            ProcessResult(consumeResult);
                            consumer.Commit(consumeResult);
                        }
                        catch (ConsumeException e)
                        {
                            _logger.LogError(e, "Error processing records");
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    consumer.Close();
                }
            }
        }

        private void Initialize()
        {
            _consumerConfig = new ConsumerConfig
            {
                BootstrapServers = _messagingConfiguration.Kafka.BootstrapServers,
                GroupId = _messagingConfiguration.Kafka.SyncBackConsumerGroupId,
                AutoOffsetReset = _messagingConfiguration.Kafka.AutoOffsetReset,
                EnableAutoCommit = false
            };

            if (_messagingConfiguration.Kafka.ConnectionType == ConnectionType.Sasl)
            {
                _consumerConfig.SecurityProtocol = SecurityProtocol.SaslSsl;
                _consumerConfig.SaslMechanism = _messagingConfiguration.Kafka.SaslMechanism;
                _consumerConfig.SaslUsername = _messagingConfiguration.Kafka.SaslUsername;
                _consumerConfig.SaslPassword = _messagingConfiguration.Kafka.SaslPassword;
                _consumerConfig.SslCaLocation = _messagingConfiguration.Kafka.CaCertLocation;
            }
            _logger.LogDebug($"Initialized: Sync Back Message Consumer");
        }

        private void ProcessResult(ConsumeResult<Null, GenericRecord> result)
        {
            try
            {
                var syncBackRecord = result.Message.Value.ParseConsumeResult<SyncBackRecord>(_schema);

                if (!syncBackRecord.IsDefault())
                {
                    _logger.LogDebug($"Sync Back record: {JsonConvert.SerializeObject(syncBackRecord)}");
                    var t = _syncBackService.SyncBackToSqlServerAsync(syncBackRecord);
                    t.Wait();
                }

                _logger.LogDebug($"ProcessResult completed");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to process message: {JsonConvert.SerializeObject(result)} --- Error: {ex}");
                //throw;
            }
        }
    }
}
