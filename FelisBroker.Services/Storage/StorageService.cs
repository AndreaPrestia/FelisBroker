using System.Collections.Concurrent;
using FelisBroker.Interfaces.Entities;
using FelisBroker.Interfaces.Services;
using Microsoft.Extensions.Logging;
using Tenray.ZoneTree;
using Tenray.ZoneTree.Comparers;
using Tenray.ZoneTree.Serializers;

namespace FelisBroker.Services.Storage;

public class StorageService : IStorage
{
    private readonly ILogger<StorageService> _logger;
    private readonly IZoneTree<string, MessageEntity> _zoneTree;
    private readonly IMaintainer _maintainer;
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private readonly ConcurrentDictionary<string, long> _offsetMap = new();
    private bool _offsetLoaded;

    public StorageService(string dataPath, ILogger<StorageService> logger)
    {
        _logger = logger;
        _zoneTree = new ZoneTreeFactory<string, MessageEntity>()
            .SetComparer(new StringInvariantIgnoreCaseComparerAscending())
            .SetDataDirectory(dataPath)
            .SetKeySerializer(new Utf8StringSerializer())
            .OpenOrCreate();
        _maintainer = _zoneTree.CreateMaintainer();
        _maintainer.EnableJobForCleaningInactiveCaches = true;
    }

    public Task<IEnumerable<long>> GetOffsetsAsync(string destination)
    {
        LoadOffsets();

        return Task.FromResult(_offsetMap.Where(pair => pair.Key == destination).Select(pair => pair.Value)
            .AsEnumerable());
    }

    public async Task<MessageEntity?> WriteMessageAsync(SourceProcessingEntity entity,
        CancellationToken cancellationToken)
    {
        try
        {
            LoadOffsets();

            await _semaphore.WaitAsync(cancellationToken);
            try
            {
                var offset = _offsetMap.AddOrUpdate(entity.Destination!, 0, (_, old) => old + 1);
                var key = BuildKey(entity.Destination!, offset);

                var message = new MessageEntity
                {
                    CorrelationId = entity.CorrelationId,
                    Offset = offset,
                    Origin = entity.Origin,
                    Destination = entity.Destination,
                    Payload = entity.Payload,
                    Timestamp = entity.Timestamp
                };

                var upsertResult = _zoneTree.Upsert(key, message);
                _logger.LogDebug(
                    "Added message '{correlationId}' for destination '{destination}' in storage with payload '{payload}' with upsertResult {upsertResult}",
                    message.CorrelationId, message.Destination, message.Payload, upsertResult);
                await _maintainer.WaitForBackgroundThreadsAsync();

                return message;
            }
            finally
            {
                _semaphore.Release();
            }
        }
        catch (Exception e)
        {
            _logger.LogError("An error '{error}' has occurred during WriteMessageAsync", e.Message);
            return null;
        }
    }

    public Task<IEnumerable<MessageEntity>> ReadMessagesAsync(string destination, int start, int end, CancellationToken ct)
    {
        if (start > end)
        {
            _logger.LogWarning("Cannot read messages from the destination '{destination}' with wrong offsets: {start} - {end}", destination, start, end);
            return Task.FromResult(Enumerable.Empty<MessageEntity>());
        }
        
        LoadOffsets();

        var results = new List<MessageEntity>();
        var prefix = destination + "|";

        var fromKey = BuildKey(destination, start);
        var toKey = BuildKey(destination, end);

        using var iterator = _zoneTree.CreateIterator();

        iterator.Seek(fromKey);

        do
        {
            var key = iterator.CurrentKey;

            if (string.CompareOrdinal(key, toKey) > 0 || !key.StartsWith(prefix))
                break;

            if (iterator.CurrentValue is not null)
                results.Add(iterator.CurrentValue);
        } while (iterator.Next());

        return Task.FromResult(results.AsEnumerable());
    }

    private void LoadOffsets()
    {
        if (_offsetLoaded) return;

        using var iterator = _zoneTree.CreateIterator();
        while (iterator.Next())
        {
            var (topic, offset) = ParseKey(iterator.CurrentKey);
            _offsetMap.AddOrUpdate(topic, offset + 1, (_, old) => Math.Max(old, offset + 1));
        }

        _offsetLoaded = true;
    }

    private static string BuildKey(string topic, long offset) =>
        $"{topic}|{offset:D10}";

    private static (string topic, long offset) ParseKey(string key)
    {
        var parts = key.Split('|');
        return (parts[0], long.Parse(parts[1]));
    }
}