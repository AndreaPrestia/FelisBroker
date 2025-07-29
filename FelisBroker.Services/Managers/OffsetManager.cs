using Microsoft.Extensions.Logging;
using Tenray.ZoneTree;
using Tenray.ZoneTree.Serializers;

namespace FelisBroker.Services.Managers;

public class OffsetManager
{
    private readonly IZoneTree<string, long> _offsetTree;
    private readonly IMaintainer _maintainer;
    private readonly SemaphoreSlim _semaphoreSlim = new(1, 1);
    private readonly ILogger<OffsetManager> _logger;

    public OffsetManager(string path, ILogger<OffsetManager> logger)
    {
        _logger = logger;
        _offsetTree = new ZoneTreeFactory<string, long>()
            .SetDataDirectory(path)
            .SetKeySerializer(new Utf8StringSerializer())
            .OpenOrCreate();

        _maintainer = _offsetTree.CreateMaintainer();
        _maintainer.EnableJobForCleaningInactiveCaches = true;
    }

    public async Task<long> GetNextOffsetAsync(string topic)
    {
        long offset = 0;
        try
        {
            await _semaphoreSlim.WaitAsync();

            using var iterator = _offsetTree.CreateIterator();

            iterator.Seek(topic);
            do
            {
                if (string.Equals(topic, iterator.Current.Key))
                {
                    offset = iterator.Current.Value;
                }
            } while (iterator.Next());

            return offset;
        }
        finally
        {
            _semaphoreSlim.Release();
        }
    }

    public async Task IncrementOffsetAsync(string topic)
    {
        var current = await GetNextOffsetAsync(topic);
        await SetOffsetDataAsync(topic, current + 1);
    }

    public async Task SetOffsetAsync(string topic, long offset)
    {
        await SetOffsetDataAsync(topic, offset);
    }

    private async Task SetOffsetDataAsync(string topic, long offset)
    {
        try
        {
            await _semaphoreSlim.WaitAsync();

            var upsertResult = _offsetTree.Upsert(topic, offset);
            _logger.LogDebug(
                "Added offset for topic '{topic}'-'{offset}' with upsertResult {upsertResult}",
                topic, offset, upsertResult);
            await _maintainer.WaitForBackgroundThreadsAsync();
        }
        finally
        {
            _semaphoreSlim.Release();
        }
    }
}