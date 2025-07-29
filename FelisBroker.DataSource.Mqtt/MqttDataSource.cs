using System.Text;
using FelisBroker.Common.Configurations;
using FelisBroker.Common.Extensions;
using FelisBroker.Interfaces.Channels;
using FelisBroker.Interfaces.Entities;
using FelisBroker.Interfaces.Sources;
using MQTTnet;
using MQTTnet.Packets;
using MQTTnet.Protocol;

namespace FelisBroker.DataSource.Mqtt;

public class MqttDataSource : IDataSource
{
    private readonly MqttConfiguration _originConfiguration;
    private readonly IMqttClient _mqttClient;
    private bool _connected;
    private readonly Lock _lock = new();
    private readonly ICollectorEventChannel _collectorEventChannel;

    public MqttDataSource(MqttConfiguration originConfiguration, ICollectorEventChannel collectorEventChannel)
    {
        _originConfiguration = originConfiguration;
        _collectorEventChannel = collectorEventChannel;
        var factory = new MqttClientFactory();
        _mqttClient = factory.CreateMqttClient();
    }

    public async Task StartAsync()
    {
        await EnsureConnectedAndSubscribedAsync();
    }

    public async Task StopAsync()
    {
        await _mqttClient.DisconnectAsync();

        lock (_lock)
        {
            _connected = false;
        }
    }

    private async Task EnsureConnectedAndSubscribedAsync()
    {
        lock (_lock)
        {
            if (_connected) return;
            _connected = true;
        }

        var builder = new MqttClientOptionsBuilder()
            .WithClientId(_originConfiguration.ClientId)
            .WithTcpServer(_originConfiguration.Host, _originConfiguration.Port)
            .WithCleanSession();

        if (!string.IsNullOrEmpty(_originConfiguration.Username) &&
            !string.IsNullOrEmpty(_originConfiguration.Password))
            builder = builder.WithCredentials(_originConfiguration.Username,
                _originConfiguration.Password);

        var options = builder.Build();

        _mqttClient.ApplicationMessageReceivedAsync += OnMessageReceivedAsync;

        _mqttClient.DisconnectedAsync += OnDisconnectedAsync;

        await _mqttClient.ConnectAsync(options);

        await _mqttClient.SubscribeAsync(new MqttClientSubscribeOptions
        {
            TopicFilters =
            [
                new MqttTopicFilter
                {
                    Topic = _originConfiguration.Topic!,
                    QualityOfServiceLevel = (MqttQualityOfServiceLevel)_originConfiguration.QoS
                }
            ]
        });
    }

    private async Task OnMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs args)
    {
        var payload = Encoding.UTF8.GetString(args.ApplicationMessage.Payload);

        var entity = new SourceProcessingEntity
        {
            CorrelationId = Guid.NewGuid(),
            Key = _originConfiguration.Destination!,
            Payload = payload,
            Origin = _originConfiguration,
            Timestamp = DateTime.UtcNow.ToUnixTimestamp()
        };

        await _collectorEventChannel.PublishAsync(entity);
    }

    private Task OnDisconnectedAsync(MqttClientDisconnectedEventArgs args)
    {
        lock (_lock)
        {
            _connected = false;
            _mqttClient.ApplicationMessageReceivedAsync -= OnMessageReceivedAsync;
        }

        return Task.CompletedTask;
    }
}