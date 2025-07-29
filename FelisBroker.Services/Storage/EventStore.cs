using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using FelisBroker.Interfaces.Entities;
using FelisBroker.Interfaces.Services;
using FelisBroker.Services.Managers;
using Microsoft.Extensions.Logging;

namespace FelisBroker.Services.Storage;

public class EventStore : IEventStore
{
    private readonly string _basePath;
    private readonly OffsetManager _offsets;
    private readonly ConcurrentDictionary<string, IStorage> _partitions = new();
    private readonly ILoggerFactory _loggerFactory;

    public EventStore(string basePath, ILoggerFactory loggerFactory)
    {
        _basePath = basePath;
        _loggerFactory = loggerFactory;
        Directory.CreateDirectory(basePath);
        _offsets = new OffsetManager(Path.Combine(basePath, "offsets"), loggerFactory.CreateLogger<OffsetManager>());
    }

    public async Task<MessageEntity?> WriteMessageAsync(SourceProcessingEntity entity, CancellationToken cancellationToken)
    {
        var offset = await _offsets.GetNextOffsetAsync(entity.Key!);
        var message = await GetPartition(entity.Key!).WriteMessageAsync(entity, cancellationToken);
        await _offsets.IncrementOffsetAsync(entity.Key!);
        return message;
    }

    public async IAsyncEnumerable<MessageEntity> ReadMessagesAsync(string key, long start, long end, [EnumeratorCancellation] CancellationToken ct = default)
    {
        var partition = GetPartition(key);

        foreach (var kv in await partition.ReadMessagesAsync(key, start, end, ct))
        {
            if (ct.IsCancellationRequested) yield break;
            yield return kv;
            await Task.Yield();
        }
    }
    
    private IStorage GetPartition(string key)
    {
        return _partitions.GetOrAdd(key, k => new StorageService(k, Path.Combine(_basePath, "keys"), _loggerFactory.CreateLogger<StorageService>()));
    }
}