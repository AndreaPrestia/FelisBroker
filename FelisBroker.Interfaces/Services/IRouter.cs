using FelisBroker.Interfaces.Entities;

namespace FelisBroker.Interfaces.Services;

public interface IRouter
{
    Task<IList<int>> GetOffsetsAsync(string topic);
    Task<IList<MessageEntity>> ReadMessagesAsync(string topic, int offset);
}