using FelisBroker.Interfaces.Channels;
using FelisBroker.Interfaces.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FelisBroker.Services.Hosted;

public class CollectorService: IHostedService
{
    private readonly ICollectorEventChannel  _collectorEventChannel;
    private readonly IStorage _storage;
    private readonly ILogger<CollectorService> _logger;

    public CollectorService(ICollectorEventChannel collectorEventChannel, IStorage storage, ILogger<CollectorService> logger)
    {
        _collectorEventChannel = collectorEventChannel;
        _storage =  storage;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await foreach (var entity in _collectorEventChannel.ReadAllAsync().WithCancellation(cancellationToken))
        {
            try
            {
                var messageEntity = await _storage.WriteMessageAsync(entity, cancellationToken);
                _logger.LogInformation($"Message received: {messageEntity}");
            }
            catch (Exception ex)
            {
                _logger.LogError("[Error] Failed to process message: {ExMessage}", ex.Message);
                // Optional: DLQ, Retry logic, etc.
            }
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}