using FelisBroker.Interfaces.Entities;

namespace FelisBroker.Interfaces.Services;

public interface ISinkProcessor
{
    Task FlushAsync(IList<MessageEntity> messages);
}