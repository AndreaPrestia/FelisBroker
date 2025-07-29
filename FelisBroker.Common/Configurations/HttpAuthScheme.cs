using System.Text.Json.Serialization;

namespace FelisBroker.Common.Configurations;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum HttpAuthScheme
{
    None,
    Basic,
    Jwt,
    MTls
}