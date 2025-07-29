using FelisBroker.Common.Configurations;

namespace FelisBroker.Interfaces.Services;

public interface IOrchestrator
{
    Task StartAsync(IList<OriginConfiguration> origins);
    Task StopAsync();
}