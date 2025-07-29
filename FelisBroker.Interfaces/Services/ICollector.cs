using FelisBroker.Common.Configurations;

namespace FelisBroker.Interfaces.Services;

public interface ICollector
{
    Task<Guid> CollectAsync(string topic, string payload, OriginConfiguration origin);
}