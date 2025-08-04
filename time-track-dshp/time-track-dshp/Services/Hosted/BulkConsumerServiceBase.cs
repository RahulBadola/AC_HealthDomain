using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using time_track_dshp.Models.Configuration;

namespace time_track_dshp.Services.Hosted
{
    public abstract class BulkConsumerServiceBase : BackgroundService
    {
        #region constructors

        protected BulkConsumerServiceBase(
            string consumerGroupId,
            string consumerTopic,
            ILogger logger,
            MessagingConfiguration messagingConfiguration,
            IBulkResultProcessor resultProcessor,
            string targetCommitBatchSizeString)
            : this(consumerGroupId, consumerTopic, logger, messagingConfiguration, resultProcessor, int.Parse(targetCommitBatchSizeString))
        { }

        protected BulkConsumerServiceBase(
            string consumerGroupId, 
            string consumerTopic,
            ILogger logger,
            MessagingConfiguration messagingConfiguration,
            IBulkResultProcessor resultProcessor,
            int targetCommitBatchSize)
        {
            _consumerGroupId = consumerGroupId ?? throw new ArgumentNullException(nameof(consumerGroupId));
            _consumerTopic = consumerTopic ?? throw new ArgumentNullException(nameof(consumerTopic));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _messagingConfiguration = messagingConfiguration ?? throw new ArgumentNullException(nameof(messagingConfiguration));
            _resultProcessor = resultProcessor;
            _targetCommitBatchSize = targetCommitBatchSize;
        }

        #endregion

        protected override Task ExecuteAsync(CancellationToken stoppingToken) => Task.Run(async () =>
        {
            _logger.LogInformation($"Initializing {_consumerTopic} subscription.");
            using var consumer = BuildConsumer();
            consumer.Subscribe(_consumerTopic);
            
            _logger.LogInformation($"Subscribed to {_consumerTopic}. Listening...");
            
            ConsumeResult<string, string> consumeResult = null;
            var offsetCommitter = new OffsetCommitter<string, string>(consumer, _targetCommitBatchSize, _logger);
            var retryTally = new Dictionary<long, int>();

            try
            {
                var collector = new List<ConsumeResult<string, string>>();
                while (true)
                {
                    try
                    {
                        consumeResult = consumer.Consume(offsetCommitter.PollTimeout);
                        if (consumeResult == null)
                        {
                            _logger.LogDebug("Awaiting messages...");
                            await _resultProcessor.ProcessBulkResult(collector); // dispatch the consumed result to the appropriate handlers
                            offsetCommitter.Commit("Kafka msg null, committing previous offset"); // commit when there are currently no new messages in the topic
                            collector.Clear();
                            continue;
                        }
                        await ProcessCollector(_resultProcessor, collector, consumeResult, offsetCommitter);
                    }
                    catch (ConsumeException e)
                    {
                        _logger.LogError(e, "Error processing records");
                    }
                    catch (Exception exc) // retry indefinitely when patient ops svc becomes unavailable; or any unhandled exception
                    {
                        offsetCommitter.Commit("Error occurred, committing previous offset");
                        HandleException(exc, consumeResult, retryTally);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                consumer.Close();
            }
        }, stoppingToken);

        #region private

        private readonly string _consumerGroupId;
        private readonly string _consumerTopic;
        private readonly ILogger _logger;
        private readonly MessagingConfiguration _messagingConfiguration;
        private readonly IBulkResultProcessor _resultProcessor;
        private readonly int _targetCommitBatchSize;

        private IConsumer<string, string> BuildConsumer()
        {
            var consumerConfig = CreateConsumerConfig();
            var builder = new ConsumerBuilder<string, string>(consumerConfig);
            return builder.Build();
        }

        private ConsumerConfig CreateConsumerConfig()
        {
            var consumerConfig = new ConsumerConfig
            {
                BootstrapServers = _messagingConfiguration.Kafka.BootstrapServers,
                GroupId = _consumerGroupId,
                AutoOffsetReset = _messagingConfiguration.Kafka.AutoOffsetReset,
                SessionTimeoutMs = 180000, // 3 minutes; allows time for offset commits due to intermittent message lull or transient asset (Mongo, SQL) failure
                EnableAutoCommit = false // require an explicit acknowledgment in order to recover from [hopefully-]transient failure; https://docs.confluent.io/platform/current/clients/consumer.html#offset-management
            };

            if (_messagingConfiguration.Kafka.ConnectionType != ConnectionType.Sasl) return consumerConfig;

            consumerConfig.SecurityProtocol = SecurityProtocol.SaslSsl;
            consumerConfig.SaslMechanism = _messagingConfiguration.Kafka.SaslMechanism;
            consumerConfig.SaslUsername = _messagingConfiguration.Kafka.SaslUsername;
            consumerConfig.SaslPassword = _messagingConfiguration.Kafka.SaslPassword;
            consumerConfig.SslCaLocation = _messagingConfiguration.Kafka.CaCertLocation;

            return consumerConfig;
        }

        private void HandleException(Exception ex, ConsumeResult<string, string> consumeResult, Dictionary<long, int> retryTally)
        {
            if (consumeResult != null)
            {
                var attemptCount = retryTally.GetValueOrDefault(consumeResult.Offset.Value) + 1; // 1-indexed
                var retryMessage = $"Retrying offset {consumeResult.Offset.Value} in 10 seconds.";
                if (attemptCount == 1) _logger.LogCritical(ex, retryMessage);
                if (attemptCount > 1) _logger.LogCritical($"{ex.Message} {retryMessage}"); // let's avoid flooding the logs while continuing to provide context.
                retryTally[consumeResult.Offset.Value] = attemptCount;
                Thread.Sleep(10000); // sleep for 10 seconds

                _logger.LogCritical($"Retrying offset: offset:{consumeResult.Offset.Value} attempt:{attemptCount}");
            }
            else
            {
                _logger.LogCritical(ex, "consumeResult not found");
            }
        }

        private async Task ProcessCollector(
            IBulkResultProcessor _bulkResultProcessor, 
            List<ConsumeResult<string, string>> collector, 
            ConsumeResult<string, string> consumeResult, 
            OffsetCommitter<string, string> offsetCommitter)
        {
            collector.Add(consumeResult);
            if (collector.Count >= _targetCommitBatchSize)
            {
                _logger.LogDebug($"Processing offset {consumeResult.Offset.Value}.");
                await _bulkResultProcessor.ProcessBulkResult(collector); // dispatch the consumed result to the appropriate handlers
                offsetCommitter.MaybeCommit(consumeResult);
                collector.Clear();
            }
        }

        #endregion
    }
}
