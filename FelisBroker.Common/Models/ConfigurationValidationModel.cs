using FelisBroker.Common.Configurations;

namespace FelisBroker.Common.Models;

public record ConfigurationValidationModel(OriginType Type, bool Success, List<ConfigurationValidationPropertyModel> Properties)
{
    public override string ToString()
    {
        return $"Validation of configuration: {Type} status: {Success}. {Environment.NewLine} Properties: {Environment.NewLine} {string.Join(Environment.NewLine, Properties.Select(p => $"{p.Property}:{p.Message}"))}";
    }
};

public record ConfigurationValidationPropertyModel(string Property, string Message);
