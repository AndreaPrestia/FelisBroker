using FelisBroker.Common.Configurations;
using FelisBroker.DataSource.Http;
using FelisBroker.DataSource.Mqtt;
using FelisBroker.DataSource.RabbitMq;
using FelisBroker.DataSource.RedisPubSub;
using FelisBroker.Interfaces.Channels;
using FelisBroker.Interfaces.Sources;

namespace FelisBroker.Services.Factories;

public static class DataSourceFactory
{
    public static IDataSource Create(OriginConfiguration config, ICollectorEventChannel channel)
    {
        return config switch
        {
            HttpConfiguration http => new HttpDataSource(http, channel),
            RabbitMqConfiguration rabbitMq => new RabbitMqDataSource(rabbitMq, channel),
            MqttConfiguration mqtt => new MqttDataSource(mqtt, channel),
            RedisPubSubConfiguration redis => new RedisPubSubDataSource(redis, channel),
            _ => throw new NotSupportedException($"Unsupported origin type: {config.GetType().Name}")
        };
    }
}