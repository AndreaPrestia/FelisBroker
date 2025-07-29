using FelisBroker.Interfaces.Entities;

namespace FelisBroker.Interfaces.Services;

public interface IRouter
{
    Task<IEnumerable<long>> GetOffsetsAsync(string key, CancellationToken cancellationToken);
    Task<IEnumerable<MessageEntity>> ReadMessagesAsync(string key, long start, long end, CancellationToken cancellationToken);
}