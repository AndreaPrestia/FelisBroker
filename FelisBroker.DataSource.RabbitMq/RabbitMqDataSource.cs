using FelisBroker.Common.Configurations;
using FelisBroker.Interfaces.Channels;
using FelisBroker.Interfaces.Sources;
using RabbitMQ.Client;

namespace FelisBroker.DataSource.RabbitMq;

public class RabbitMqDataSource : IDataSource
{
    private readonly RabbitMqConfiguration _rabbitMqConfiguration;
    private IConnection _connection;
    //private IModel _channelModel;
    private bool _subscribed = false;
    private readonly Lock _lock = new();
    private readonly ISourceEventChannel _eventChannel;

    public RabbitMqDataSource(RabbitMqConfiguration originConfiguration, ISourceEventChannel eventChannel)
    {
        var validation = originConfiguration.Validate();

        if (!validation.Success)
        {
            throw new ApplicationException(validation.ToString());
        }
        
        _rabbitMqConfiguration = originConfiguration;
        _eventChannel = eventChannel;

        InitializeConnection();
    }

    private void InitializeConnection()
    {
        var factory = new ConnectionFactory
        {
            Uri = new Uri(_rabbitMqConfiguration.Host ?? throw new ArgumentNullException(nameof(_rabbitMqConfiguration.Host))),
            UserName = _rabbitMqConfiguration.Username ??  throw new ArgumentNullException(nameof(_rabbitMqConfiguration.Username)),
            Password = _rabbitMqConfiguration.Password ??  throw new ArgumentNullException(nameof(_rabbitMqConfiguration.Password)),
        };

        // _connection = factory.CreateConnection();
        // _channelModel = _connection.CreateModel();
    }

    public Task StartAsync()
    {
        EnsureSubscribed();
        
        return Task.CompletedTask;
        
        // foreach (var message in _eventChannel.ReadAllAsync())
        // {
        //     
        // }
        // while (await _eventChannel.ReadAllAsync())
        // {
        //     while (_eventChannel.(out var message))
        //     {
        //         yield return message;
        //     }
        // }
    }

    public Task StopAsync()
    {
        lock (_lock)
        {
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
        var queue = _rabbitMqConfiguration.Queue;
        var exchange = _rabbitMqConfiguration.Exchange;
        var routingKey = _rabbitMqConfiguration.RoutingKey ?? string.Empty;
        var exchangeType = _rabbitMqConfiguration.ExchangeType ?? string.Empty;

        // _channelModel.QueueDeclare(queue, 
        //     durable: _rabbitMqConfiguration.Durable, 
        //     exclusive: _rabbitMqConfiguration.Exclusive, 
        //     autoDelete: _rabbitMqConfiguration.AutoDelete);
        // if (!string.IsNullOrEmpty(exchange))
        // {
        //     _channelModel.ExchangeDeclare(exchange, exchangeType, durable: _rabbitMqConfiguration.Durable);
        //     _channelModel.QueueBind(queue, exchange, routingKey);
        // }

        // var consumer = new EventingBasicConsumer(_channelModel);
        // consumer.Received += (_, ea) =>
        // {
        //     var body = ea.Body.ToArray();
        //     var message = Encoding.UTF8.GetString(body);
        //
        //     var entity = new SourceProcessingEntity
        //     {
        //         Topic = _rabbitMqConfiguration.Queue,
        //         Payload = message
        //     };
        //
        //     _channel.Writer.TryWrite(entity);
        //
        //     // Manual acknowledgment if AutoAck is false
        //     var autoAck = _rabbitMqConfiguration.AutoAck;
        //     if (!autoAck)
        //     {
        //         _channelModel.BasicAck(ea.DeliveryTag, multiple: false);
        //     }
        // };
        //
        // var consumerTag = _channelModel.BasicConsume(
        //     queue: queue,
        //     autoAck: _rabbitMqConfiguration.AutoAck,
        //     consumer: consumer
        // );
    }
}