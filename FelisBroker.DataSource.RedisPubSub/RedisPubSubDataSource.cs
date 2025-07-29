using FelisBroker.Common.Configurations;
using FelisBroker.Common.Extensions;
using FelisBroker.Interfaces.Channels;
using FelisBroker.Interfaces.Entities;
using FelisBroker.Interfaces.Sources;
using StackExchange.Redis;

namespace FelisBroker.DataSource.RedisPubSub;

public class RedisPubSubDataSource : IDataSource
{
    private readonly RedisPubSubConfiguration _originConfiguration;
    private ISubscriber? _subscriber;
    private readonly ICollectorEventChannel _collectorEventChannel;
    private bool _subscribed;
    private readonly Lock _lock = new();

    public RedisPubSubDataSource(RedisPubSubConfiguration originConfiguration, ICollectorEventChannel collectorEventChannel)
    {
        _originConfiguration = originConfiguration;
        _collectorEventChannel = collectorEventChannel;
    }

    public Task StartAsync()
    {
        EnsureSubscribed();
        return Task.CompletedTask;
    }

    public Task StopAsync()
    {
        lock (_lock)
        {
            _subscriber?.UnsubscribeAll();
            _subscribed = false;
        }
        
        return Task.CompletedTask;
    }

    private void EnsureSubscribed()
    {
        lock (_lock)
        {
            if (_subscribed) return;
            Subscribe();
            _subscribed = true;
        }
    }

    private void Subscribe()
    {
        if (_subscriber == null || !_subscriber.IsConnected())
        {
            var connection = ConnectionMultiplexer.Connect(_originConfiguration.ConnectionString!);
            _subscriber = connection.GetSubscriber();
        }
        
        _subscriber.Subscribe(
            new RedisChannel(_originConfiguration.ChannelName!, RedisChannel.PatternMode.Auto),
            (_, message) =>
            {
                var entity = new SourceProcessingEntity
                {
                    CorrelationId = Guid.NewGuid(),
                    Key = _originConfiguration.Destination!,
                    Payload = message.ToString(),
                    Origin = _originConfiguration,
                    Timestamp = DateTime.UtcNow.ToUnixTimestamp()
                };

                _collectorEventChannel.PublishAsync(entity);
            });
    }
}