using FelisBroker.Interfaces.Entities;

namespace FelisBroker.Interfaces.Services;

public interface IRouter
{
    Task<IList<int>> GetOffsetsAsync(string destination);
    Task<IList<MessageEntity>> ReadMessagesAsync(string destination, int start, int end);
}