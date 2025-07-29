using FelisBroker.Common.Configurations;
using FelisBroker.Interfaces.Channels;
using FelisBroker.Interfaces.Entities;
using FelisBroker.Interfaces.Sources;
using StackExchange.Redis;

namespace FelisBroker.DataSource.RedisPubSub;

public class RedisPubSubDataSource : IDataSource
{
    private readonly RedisPubSubConfiguration _originConfiguration;
    private readonly ISubscriber _subscriber;
    private readonly ISourceEventChannel _sourceEventChannel;
    private bool _subscribed;
    private readonly Lock _lock = new();

    public RedisPubSubDataSource(RedisPubSubConfiguration originConfiguration, ISourceEventChannel sourceEventChannel)
    {
        var validation = originConfiguration.Validate();

        if (!validation.Success)
        {
            throw new ApplicationException(validation.ToString());
        }
        
        _originConfiguration = originConfiguration;
        _sourceEventChannel = sourceEventChannel;
        var connection = ConnectionMultiplexer.Connect(_originConfiguration.ConnectionString!);
        _subscriber = connection.GetSubscriber();
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
            _subscriber.UnsubscribeAll();
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
        _subscriber.Subscribe(
            new RedisChannel(_originConfiguration.ChannelName!, RedisChannel.PatternMode.Auto),
            (_, message) =>
            {
                var entity = new SourceProcessingEntity
                {
                    Destination = _originConfiguration.Destination!,
                    Payload = message.ToString()
                };

                _sourceEventChannel.PublishAsync(entity);
            });
    }
}