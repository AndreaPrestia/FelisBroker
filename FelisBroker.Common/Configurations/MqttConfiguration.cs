using System.ComponentModel.DataAnnotations;
using FelisBroker.Common.Helpers;

namespace FelisBroker.Common.Configurations;

public class MqttConfiguration : OriginConfiguration
{
    [Required(AllowEmptyStrings = false)]
    public string? Topic { get; init; }
    [Required(AllowEmptyStrings = false)]
    public string? Host { get; init; }
    public int Port { get; init; }
    [Required(AllowEmptyStrings = false)]
    public string? ClientId { get; init; }
    [Required(AllowEmptyStrings = false)]
    public string? Username { get; init; }
    public string? Password { get; init; }
    [Range(0, 2)]
    public int QoS { get; init; }
    
    public override ValidationResponse Validate()
    {
        var results = new List<ValidationResult>();
        var context = new ValidationContext(this);

        bool isValid = Validator.TryValidateObject(this, context, results, validateAllProperties: true);

        return isValid
            ? ValidationResponse.Ok()
            : ValidationResponse.Ko(results.Select(r => r.ErrorMessage!).ToArray());
    }
}