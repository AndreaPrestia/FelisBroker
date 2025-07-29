using FelisBroker.Common.Helpers;

namespace FelisBroker.Common.Configurations;

public abstract class OriginConfiguration
{
    public string? Name { get; set; }
    public string? Destination { get; set; }
    public OriginType Type { get; set; }
    public abstract ValidationResponse Validate();
}