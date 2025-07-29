using FelisBroker.Interfaces.Entities;

namespace FelisBroker.Interfaces.Services;

public interface IStorage
{
    Task<IList<int>> GetOffsetsAsync(string topic);
    Task<bool> WriteMessageAsync(MessageEntity message);
    Task<IList<MessageEntity>> ReadMessagesAsync(string topic, int offset);
}