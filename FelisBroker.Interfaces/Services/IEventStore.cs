using FelisBroker.Interfaces.Entities;

namespace FelisBroker.Interfaces.Services;

public interface IEventStore
{
    Task<MessageEntity?> WriteMessageAsync(SourceProcessingEntity entity, CancellationToken cancellationToken);
    IAsyncEnumerable<MessageEntity> ReadMessagesAsync(string key, long start, long end, CancellationToken cancellationToken);
}