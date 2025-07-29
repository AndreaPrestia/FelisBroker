using FelisBroker.Common.Configurations;

namespace FelisBroker.Interfaces.Entities;

public class MessageEntity
{
    public Guid CorrelationId { get; set; }
    public string? Topic { get; set; }
    public string? Payload { get; set; }
    public long Timestamp { get; set; }
    public OriginConfiguration? Origin { get; set; } 
}