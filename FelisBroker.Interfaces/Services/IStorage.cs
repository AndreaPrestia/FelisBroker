using System.Runtime.CompilerServices;
using FelisBroker.Interfaces.Entities;

namespace FelisBroker.Interfaces.Services;

public interface IStorage
{
    Task<IEnumerable<long>> GetOffsetsAsync(string destination);
    Task<MessageEntity?> WriteMessageAsync(SourceProcessingEntity entity, CancellationToken cancellationToken);
    Task<IEnumerable<MessageEntity>> ReadMessagesAsync(string destination, int start, int end, CancellationToken cancellationToken);
}