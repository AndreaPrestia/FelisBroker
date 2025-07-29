using System.ComponentModel.DataAnnotations;
using FelisBroker.Common.Helpers;

namespace FelisBroker.Common.Configurations;

public class RabbitMqConfiguration : OriginConfiguration
{
    [Required(AllowEmptyStrings = false)]
    public string? Host { get; init; }
    [Required(AllowEmptyStrings = false)]
    public string? Username { get; init; }
    [Required(AllowEmptyStrings = false)]
    public string? Password { get; init; }
    [Required(AllowEmptyStrings = false)]
    public string? Queue { get; init; }
    [Required(AllowEmptyStrings = false)]
    public string? Exchange { get; init; }
    [Required(AllowEmptyStrings = false)]
    public string? ExchangeType { get; init; }
    public string? RoutingKey { get; init; }
    public int PrefetchCount { get; init; }
    public bool AutoAck { get; init; }
    public bool Durable { get; init; }
    public bool Exclusive { get; init; }
    public bool AutoDelete { get; init; }
    
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