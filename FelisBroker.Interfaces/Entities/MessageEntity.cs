using System.Text.Json;
using FelisBroker.Common.Configurations;

namespace FelisBroker.Interfaces.Entities;

public class MessageEntity
{
    public Guid CorrelationId { get; init; }
    public string? Destination { get; init; }
    public string? Payload { get; init; }
    public long Timestamp { get; init; }
    public OriginConfiguration? Origin { get; init; } 
    public long Offset { get; init; }

    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}