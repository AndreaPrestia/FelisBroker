using System.Text.Json;
using FelisBroker.Collector.Channels;
using FelisBroker.Common.Configurations;
using FelisBroker.Common.Converters;
using FelisBroker.Interfaces.Channels;
using FelisBroker.Interfaces.Sources;
using FelisBroker.Services.Factories;
using FelisBroker.Services.Hosted;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FelisBroker.Services.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddFelisBroker(this IServiceCollection services)
    {
        services.AddSingleton<ISourceEventChannel, SourceEventChannel>();

        var json = File.ReadAllText("origins.json");
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new OriginConfigurationConverter() }
        };

        var config = JsonSerializer.Deserialize<AppConfig>(json, options)!;
        services.AddSingleton(config);

        foreach (var origin in config.Origins)
        {
            services.AddSingleton<IDataSource>(sp =>
            {
                var channel = sp.GetRequiredService<ISourceEventChannel>();
                return DataSourceFactory.Create(origin, channel);
            });
        }

        services.AddHostedService<DataSourceStarter>();
    }
}