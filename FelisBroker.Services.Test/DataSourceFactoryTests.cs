using FelisBroker.Common.Configurations;
using FelisBroker.DataSource.Mqtt;
using FelisBroker.DataSource.RedisPubSub;
using FelisBroker.Services.Channels;
using FelisBroker.Services.Factories;

namespace FelisBroker.Services.Test;

public class DataSourceFactoryTests
{
    [Fact]
    public void Factory_Should_Create_MqttDataSource_When_Config_Is_Mqtt()
    {
        // Arrange
        var config = new MqttConfiguration
        {
            Name = "mqtt-1",
            Topic = "test/topic",
            Host = "localhost",
            Port = 1883,
            ClientId = "client-1"
        };

        var channel = new CollectorEventChannel();

        // Act
        var dataSource = DataSourceFactory.Create(config, channel);

        // Assert
        Assert.IsType<MqttDataSource>(dataSource);
    }

    [Fact]
    public void Factory_Should_Create_RedisPubSubDataSource_When_Config_Is_Redis()
    {
        // Arrange
        var config = new RedisPubSubConfiguration
        {
            ConnectionString = "localhost:6379",
            ChannelName = "events:*"
        };

        var channel = new CollectorEventChannel();

        // Act
        var dataSource = DataSourceFactory.Create(config, channel);

        // Assert
        Assert.IsType<RedisPubSubDataSource>(dataSource);
    }
}