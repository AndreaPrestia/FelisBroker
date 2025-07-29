using System.Text.Json;
using System.Text.Json.Serialization;
using FelisBroker.Common.Configurations;

namespace FelisBroker.Common.Converters;

public class OriginConfigurationConverter : JsonConverter<OriginConfiguration>
{
    public override OriginConfiguration? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var doc = JsonDocument.ParseValue(ref reader);
        var root = doc.RootElement;

        if (!root.TryGetProperty("OriginType", out var originTypeProp))
            throw new JsonException("Missing OriginType");

        var originType = Enum.Parse<OriginType>(originTypeProp.GetString()!, ignoreCase: true);

        return originType switch
        {
            OriginType.RabbitMq => JsonSerializer.Deserialize<RabbitMqConfiguration>(root.GetRawText(), options),
            OriginType.Mqtt => JsonSerializer.Deserialize<MqttConfiguration>(root.GetRawText(), options),
            OriginType.RedisPubSub => JsonSerializer.Deserialize<RedisPubSubConfiguration>(root.GetRawText(), options),
            _ => throw new JsonException($"Unknown OriginType: {originType}")
        };
    }

    public override void Write(Utf8JsonWriter writer, OriginConfiguration value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value, value.GetType(), options);
    }
}