using FelisBroker.Interfaces.Entities;
using FelisBroker.Interfaces.Services;

namespace FelisBroker.Services.Routers;

public class HttpEventRouter : IRouter
{
    private readonly IStorage _storage;

    public HttpEventRouter(IStorage storage)
    {
        _storage = storage;
    }

    public Task<IEnumerable<long>> GetOffsetsAsync(string key, CancellationToken cancellationToken) =>
        _storage.GetOffsetsAsync(key, cancellationToken);

    public Task<IEnumerable<MessageEntity>> ReadMessagesAsync(string key, long start, long end, CancellationToken cancellationToken) => _storage.ReadMessagesAsync(key, start, end, cancellationToken);
}