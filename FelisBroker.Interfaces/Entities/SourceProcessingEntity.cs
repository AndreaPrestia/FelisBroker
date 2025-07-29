using FelisBroker.Common.Configurations;

namespace FelisBroker.Interfaces.Entities;

public class SourceProcessingEntity
{
    public Guid CorrelationId { get; init; }
    public string? Key { get; init; }
    public string? Payload { get; init; }
    public OriginConfiguration? Origin { get; init; }
    public long Timestamp { get; init; }
}