using System.ComponentModel.DataAnnotations;
using FelisBroker.Common.Helpers;

namespace FelisBroker.Common.Configurations;

public class HttpConfiguration : OriginConfiguration
{
    public HttpAuthConfiguration? AuthConfiguration { get; set; }

    public override ValidationResponse Validate()
    {
        var results = new List<ValidationResult>();
        var context = new ValidationContext(this);

        var isValid = Validator.TryValidateObject(this, context, results, validateAllProperties: true);

        return isValid
            ? ValidationResponse.Ok()
            : ValidationResponse.Ko(results.Select(r => r.ErrorMessage!).ToArray());
    }
}