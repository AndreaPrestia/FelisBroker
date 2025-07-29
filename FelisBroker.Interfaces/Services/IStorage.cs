using System.Runtime.CompilerServices;
using FelisBroker.Interfaces.Entities;

namespace FelisBroker.Interfaces.Services;

public interface IStorage
{
    Task<IEnumerable<long>> GetOffsetsAsync(string key, CancellationToken cancellationToken);
    Task<MessageEntity?> WriteMessageAsync(SourceProcessingEntity entity, CancellationToken cancellationToken);
    Task<IEnumerable<MessageEntity>> ReadMessagesAsync(string key, long start, long end, CancellationToken cancellationToken);
}