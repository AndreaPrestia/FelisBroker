using FelisBroker.Interfaces.Sources;
using Microsoft.Extensions.Hosting;

namespace FelisBroker.Services.Hosted;

public class DataSourceStarter : IHostedService
{
    private readonly IEnumerable<IDataSource> _dataSources;

    public DataSourceStarter(IEnumerable<IDataSource> dataSources)
    {
        _dataSources = dataSources;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        foreach (var source in _dataSources)
        {
            await source.StartAsync();
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        foreach (var source in _dataSources)
        {
            await source.StopAsync();
        }
    }
}
