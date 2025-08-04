using System;
using Confluent.Kafka;

namespace time_track_dshp.Models.Configuration
{
    public class KafkaConfiguration
    {
        #region AssureCare Config Options
        /// <summary>
        /// Type of Connection.
        /// </summary>
        public ConnectionType ConnectionType { get; set; }


        public string HydrationConsumerGroupId { get; set; }

        public string SyncBackConsumerGroupId { get; set; }
        private string _syncBackConsumerTopic;
        public string SyncBackConsumerTopic
        {
            get => _syncBackConsumerTopic;
            set => _syncBackConsumerTopic = Environment.ExpandEnvironmentVariables(value);
        }
        public string SyncBackConsumerSchemaFileLocation { get; set; }
        private string _syncBackProducerTopic;
        public string SyncBackProducerTopic
        {
            get => _syncBackProducerTopic;
            set => _syncBackProducerTopic = Environment.ExpandEnvironmentVariables(value);
        }
        public string SyncBackProducerSchemaFileLocation { get; set; }

        private string _hydrationBulkConsumerTopic;
        public string HydrationBulkConsumerTopic
        {
            get => _hydrationBulkConsumerTopic;
            set => _hydrationBulkConsumerTopic = Environment.ExpandEnvironmentVariables(value);
        }

        private string _targetCommitBatchSize;
        public string TargetCommitBatchSize
        {
            get => _targetCommitBatchSize;
            set => _targetCommitBatchSize = Environment.ExpandEnvironmentVariables(value);
        }

        #endregion

        #region Passthrough Kafka or Confluent Configuration Options
        /// <summary>
        /// Bootstrap Servers for Kafka connection. Required.
        /// </summary>
        public string BootstrapServers { get; set; }

        /// <summary>
        /// The Sasl mechanism used to connect to the Kafka instance.
        /// Required if Connection Type is Sasl
        /// </summary>
        public SaslMechanism SaslMechanism { get; set; } = SaslMechanism.Plain;

        /// <summary>
        /// Username to connect to the Kafka instance.
        /// Required if Connection Type is Sasl
        /// For Confluent connections, this is the API Gateway Key
        /// </summary>
        public string SaslUsername { get; set; }

        /// <summary>
        /// Username to connect to the Kafka instance.
        /// Required if Connection Type is Sasl
        /// For Confluent connections, this is the API Gateway Secret
        /// </summary>
        public string SaslPassword { get; set; }

        public AutoOffsetReset AutoOffsetReset { get; set; } = AutoOffsetReset.Latest;

        /// <summary>
        /// Bundle of certificates. This can be extracted from the
        /// Mozilla Firefox Browser or directly from this location.
        ///
        ///     https://curl.haxx.se/docs/caextract.html
        /// </summary>
        public string CaCertLocation { get; set; }

        #endregion



    }

    public enum ConnectionType
    {
        Insecure,
        Sasl
    }
}