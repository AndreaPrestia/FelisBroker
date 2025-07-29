using System.Threading.Channels;
using FelisBroker.Interfaces.Channels;
using FelisBroker.Interfaces.Entities;

namespace FelisBroker.Services.Channels;

public class CollectorEventChannel : ICollectorEventChannel
{
    private readonly Channel<SourceProcessingEntity> _channel = Channel.CreateUnbounded<SourceProcessingEntity>();

    public async Task PublishAsync(SourceProcessingEntity entity)
    {
        await _channel.Writer.WriteAsync(entity);
    }

    public async IAsyncEnumerable<SourceProcessingEntity> ReadAllAsync()
    {
        while (await _channel.Reader.WaitToReadAsync())
        {
            while (_channel.Reader.TryRead(out var msg))
            {
                yield return msg;
            }
        }
    }
}