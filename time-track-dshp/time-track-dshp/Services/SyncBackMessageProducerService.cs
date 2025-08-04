using Avro.Generic;
using Confluent.Kafka;
using Confluent.SchemaRegistry.Serdes;
using time_track_dshp.Models.Configuration;
using time_track_dshp.Utils.Serialization.Confluent;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace time_track_dshp.Services
{
    public class SyncBackMessageProducerService : ISyncBackMessageProducerService
    {
        private readonly ILogger<SyncBackMessageProducerService> _logger;
        private readonly MessagingConfiguration _messagingConfiguration;
        private readonly string[] _topics;

        private IProducer<Null, GenericRecord> _producer;

        public SyncBackMessageProducerService(ILogger<SyncBackMessageProducerService> logger, MessagingConfiguration messagingConfiguration)
        {
            _logger = logger;
            _messagingConfiguration = messagingConfiguration;
            _topics = _messagingConfiguration.Kafka.SyncBackProducerTopic.Split(",");

            _logger.LogInformation($"Kafka Topic(s): {_messagingConfiguration.Kafka.SyncBackProducerTopic}");
            _logger.LogInformation("Intialize Message Producer");
            Initialize();
            _logger.LogInformation("Message Producer Initialized");
        }

        /// <summary>
        /// Initializes a producer instance
        /// </summary>
        public void Initialize()
        {
            var schema = File.ReadAllText(_messagingConfiguration.Kafka.SyncBackProducerSchemaFileLocation, Encoding.UTF8);
            _logger.LogDebug($"Schema\n{schema}");

            var producerConfig = new ProducerConfig
            {
                BootstrapServers = _messagingConfiguration.Kafka.BootstrapServers,
                EnableIdempotence = true,
                MessageSendMaxRetries = 20, // Set high in order to wait through a temporary connectivity loss 
                MessageTimeoutMs = 20000 // Set high in order to wait through a temporary connectivity loss 
            };

            if (_messagingConfiguration.Kafka.ConnectionType == ConnectionType.Sasl)
            {
                producerConfig.SecurityProtocol = SecurityProtocol.SaslSsl;
                producerConfig.SaslMechanism = _messagingConfiguration.Kafka.SaslMechanism;
                producerConfig.SaslUsername = _messagingConfiguration.Kafka.SaslUsername;
                producerConfig.SaslPassword = _messagingConfiguration.Kafka.SaslPassword;
                producerConfig.SslCaLocation = _messagingConfiguration.Kafka.CaCertLocation;
            }

            _producer = new ProducerBuilder<Null, GenericRecord>(producerConfig)
                .SetValueSerializer(new AvroSerializer<GenericRecord>(new LocalSchemaRegistryClient(schema)))
                .Build();
        }

        /// <summary>
        /// Produces a message to a Kafka topic.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task ProduceMessageAsync(GenericRecord message)
        {
            foreach (var topic in _topics)
            {
                try
                {
                    await _producer.ProduceAsync(topic, new Message<Null, GenericRecord> { Value = message });
                }
                //These types of errors occur when the requested topic is not defined
                //Or there is an issue reaching the Kafka broker
                catch (Exception e)
                {
                    _logger.LogError($"Error:", e.InnerException);
                    if (e.InnerException != null)
                    {
                        _logger.LogError($"Inner Exception: {e.InnerException.Message}", e.InnerException);
                    }
                    throw;
                }
            }
        }

        /// <summary>
        /// Flushes any remaining objects in the producer cache.
        /// </summary>
        public void FlushProducer()
        {
            _logger.LogInformation("Flushing Producer");
            _producer.Flush();
        }
    }
}
