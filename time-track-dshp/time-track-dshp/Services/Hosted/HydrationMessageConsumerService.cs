using Avro;
using time_track_dshp.Models.Configuration;
using time_track_dshp.Services.Hosted.ResultProcessors;
using Microsoft.Extensions.Logging;
using System.IO;

namespace time_track_dshp.Services.Hosted
{
    /// <summary>
    /// Main application logic for the app. Processes the consumer result.
    /// </summary>
    public class HydrationMessageConsumerService : BulkConsumerServiceBase
    {
        public HydrationMessageConsumerService(
            IHydrationService hydrationService,
            ILogger<HydrationMessageConsumerService> logger,
            MessagingConfiguration messagingConfiguration) : 
            base(
                consumerGroupId: messagingConfiguration.Kafka.HydrationConsumerGroupId,
                consumerTopic: messagingConfiguration.Kafka.HydrationBulkConsumerTopic,
                logger: logger,
                messagingConfiguration: messagingConfiguration, 
                resultProcessor: new HydrationResultProcessor(logger, hydrationService),
                messagingConfiguration.Kafka.TargetCommitBatchSize)
        { }
    }
}