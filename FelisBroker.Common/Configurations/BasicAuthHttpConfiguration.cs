using System.ComponentModel.DataAnnotations;

namespace FelisBroker.Common.Configurations;

public class BasicAuthHttpConfiguration : HttpAuthConfiguration
{
    [Required(AllowEmptyStrings = false)]
    public string? Username { get; init; }
    [Required(AllowEmptyStrings = false)]
    public string? Password { get; init; }
}