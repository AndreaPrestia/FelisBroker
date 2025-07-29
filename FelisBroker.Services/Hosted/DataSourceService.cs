using FelisBroker.Interfaces.Sources;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FelisBroker.Services.Hosted;

public class DataSourceService : IHostedService
{
    private readonly IEnumerable<IDataSource> _dataSources;
    private readonly ILogger<DataSourceService> _logger;

    public DataSourceService(IEnumerable<IDataSource> dataSources, ILogger<DataSourceService> logger)
    {
        _dataSources = dataSources;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        foreach (var source in _dataSources)
        {
            try
            {
                await source.StartAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError("[Error] Failed to process source: {ExMessage}", ex.Message);
            }
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
