using System.Text.Json.Serialization;

namespace FelisBroker.Common.Configurations;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum OriginType
{
    Http, 
    RabbitMq, 
    Kafka, 
    AmazonSqs, 
    AmazonSns, 
    OracleOci, 
    Mqtt, 
    RedisPubSub, 
    FsWatch
}