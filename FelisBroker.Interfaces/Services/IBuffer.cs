using FelisBroker.Interfaces.Entities;

namespace FelisBroker.Interfaces.Services;

public interface IBuffer
{
    Task<bool> AppendAsync(MessageEntity message);
    Task<bool> FlushAsync();
}