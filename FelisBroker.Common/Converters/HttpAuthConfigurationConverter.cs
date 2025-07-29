using System.Text.Json;
using System.Text.Json.Serialization;
using FelisBroker.Common.Configurations;

namespace FelisBroker.Common.Converters;

public class HttpAuthConfigurationConverter : JsonConverter<HttpAuthConfiguration>
{
    public override HttpAuthConfiguration? Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        using var doc = JsonDocument.ParseValue(ref reader);
        var root = doc.RootElement;

        if (!root.TryGetProperty("AuthScheme", out var authSchemeProp))
            throw new JsonException("Missing AuthScheme");

        var authScheme = Enum.Parse<HttpAuthScheme>(authSchemeProp.GetString()!, ignoreCase: true);

        return authScheme switch
        {
            HttpAuthScheme.Basic => JsonSerializer.Deserialize<BasicAuthHttpConfiguration>(root.GetRawText(), options),
            HttpAuthScheme.Jwt => JsonSerializer.Deserialize<JwtAuthHttpConfiguration>(root.GetRawText(), options),
            HttpAuthScheme.MTls => JsonSerializer.Deserialize<MTlsAuthHttpConfiguration>(root.GetRawText(), options),
            HttpAuthScheme.None => null,
            _ => throw new JsonException($"Unknown AuthScheme: {authScheme}")
        };
    }

    public override void Write(Utf8JsonWriter writer, HttpAuthConfiguration value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value, value.GetType(), options);
    }
}