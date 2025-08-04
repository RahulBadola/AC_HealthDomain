using Confluent.Kafka;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace time_track_dshp.Services.Hosted
{
    public interface IBulkResultProcessor
    {
        Task ProcessBulkResult(IEnumerable<ConsumeResult<string, string>> consumeResults);
    }
}