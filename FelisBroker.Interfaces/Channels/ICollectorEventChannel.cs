using FelisBroker.Interfaces.Entities;

namespace FelisBroker.Interfaces.Channels;

public interface ICollectorEventChannel
{
    Task PublishAsync(SourceProcessingEntity entity);

    IAsyncEnumerable<SourceProcessingEntity> ReadAllAsync();
}