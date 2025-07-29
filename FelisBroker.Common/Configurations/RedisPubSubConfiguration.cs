using System.ComponentModel.DataAnnotations;
using FelisBroker.Common.Helpers;

namespace FelisBroker.Common.Configurations;

public class RedisPubSubConfiguration : OriginConfiguration
{
    [Required(AllowEmptyStrings = false)]
    public string? ConnectionString { get; init; }
    [Required(AllowEmptyStrings = false)]
    public string? ChannelName { get; init; }

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