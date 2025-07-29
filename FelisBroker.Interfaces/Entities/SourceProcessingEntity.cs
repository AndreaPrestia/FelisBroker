using FelisBroker.Common.Configurations;

namespace FelisBroker.Interfaces.Entities;

public class SourceProcessingEntity
{
    public string? Destination { get; init; }
    public string? Payload { get; init; }
    public OriginConfiguration? Origin { get; init; }
}