using FelisBroker.Interfaces.Entities;

namespace FelisBroker.Interfaces.Channels;

public interface ISourceEventChannel
{
    Task PublishAsync(SourceProcessingEntity entity);

    IAsyncEnumerable<SourceProcessingEntity> ReadAllAsync();
}