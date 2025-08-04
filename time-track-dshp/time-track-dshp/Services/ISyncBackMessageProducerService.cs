using Avro.Generic;
using System.Threading.Tasks;

namespace time_track_dshp.Services
{
    public interface ISyncBackMessageProducerService
    {
        void Initialize();

        Task ProduceMessageAsync(GenericRecord message);

        void FlushProducer();
    }
}
