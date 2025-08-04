using Confluent.Kafka;
using time_track_dshp.Extensions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace time_track_dshp.Services.Hosted
{
    public class OffsetCommitter<TConsumerKey, TConsumerValue>
    {
        public OffsetCommitter(
            IConsumer<TConsumerKey, TConsumerValue> consumer,
            int targetCommitBatchSize,
            ILogger logger)
        {
            _consumer = consumer ?? throw new ArgumentNullException(nameof(consumer));
            _targetCommitBatchSize = targetCommitBatchSize;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public TimeSpan PollTimeout { get; set; } = TimeSpan.FromSeconds(1);

        /// <summary>
        /// Commits the most recent, successfully processed event message, if one exists.
        /// </summary>
        public bool Commit(string logInfoMessage = null)
        {
            if (_lastSuccessfullyProcessedOffset == null) return false;
            if (!string.IsNullOrWhiteSpace(logInfoMessage)) _logger.LogInformation(logInfoMessage);
            try // shouldn't let commit bring the background service thread to its knees -- no zombies here!
            {
                _consumer.Commit(new List<TopicPartitionOffset>(new[] { _lastSuccessfullyProcessedOffset }));
                _logger.LogInformation($"Offset committed: topic:{_lastSuccessfullyProcessedOffset.Topic} offset: {_lastSuccessfullyProcessedOffset.Offset.Value}");
                return true;
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Kafka commit failed");
                return false;
            }
            finally
            {
                _offsetCount = 0;
                _lastSuccessfullyProcessedOffset = null;
                PollTimeout = TimeSpan.FromSeconds(60); // nothing to commit, wait a whole minute when polling
            }
        }

        /// <summary>
        /// Records the most recent, successfully processed offset
        /// in the topic. If the commit threshold has been met, we commit.
        /// </summary>
        public void MaybeCommit(ConsumeResult<TConsumerKey, TConsumerValue> consumeResult)
        {
            SetLatestSuccessfullyProcessedOffset(consumeResult);
            if (CheckBatchThreshold())
                Commit();
        }


        private readonly IConsumer<TConsumerKey, TConsumerValue> _consumer;
        private TopicPartitionOffset _lastSuccessfullyProcessedOffset;
        private readonly ILogger _logger;
        private int _offsetCount;
        private readonly int _targetCommitBatchSize;

        private bool CheckBatchThreshold() => _offsetCount >= _targetCommitBatchSize;

        private void SetLatestSuccessfullyProcessedOffset(ConsumeResult<TConsumerKey, TConsumerValue> consumeResult)
        {
            if (consumeResult == null) return;
            _offsetCount++;
            _lastSuccessfullyProcessedOffset = new TopicPartitionOffset(
                consumeResult.TopicPartitionOffset.Topic,
                consumeResult.TopicPartitionOffset.Partition,
                consumeResult.TopicPartitionOffset.Offset);
            PollTimeout = TimeSpan.FromMilliseconds(500); // poll for only 500ms b/c we now have at least 1 successfully processed message to commit
        }
    }
}