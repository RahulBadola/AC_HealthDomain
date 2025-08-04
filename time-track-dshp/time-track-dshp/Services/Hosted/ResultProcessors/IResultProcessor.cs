using Avro.Generic;
using Confluent.Kafka;
using System.Threading.Tasks;

namespace time_track_dshp.Services.Hosted
{
    public interface IResultProcessor
    {
        Task ProcessResult(ConsumeResult<string, GenericRecord> consumeResult);
    }
}