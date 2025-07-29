using System.ComponentModel.DataAnnotations;

namespace FelisBroker.Common.Configurations;

public class JwtAuthHttpConfiguration : HttpAuthConfiguration
{
    [Required(AllowEmptyStrings = false)]
    public string? Issuer { get; init; }

    [Required(AllowEmptyStrings = false)]
    public string? Audience { get; init; }

    [Required(AllowEmptyStrings = false)]
    public string? Secret { get; init; }
    
    [Range(1, int.MaxValue)]
    public int DurationInMinutes { get; init; }

    public bool RequireHttps { get; init; } = true;
}