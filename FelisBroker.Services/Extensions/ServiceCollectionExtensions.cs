using System.Text.Json;
using FelisBroker.Common.Configurations;
using FelisBroker.Common.Converters;
using FelisBroker.Interfaces.Channels;
using FelisBroker.Interfaces.Services;
using FelisBroker.Interfaces.Sources;
using FelisBroker.Services.Channels;
using FelisBroker.Services.Factories;
using FelisBroker.Services.Hosted;
using FelisBroker.Services.Middlewares;
using FelisBroker.Services.Storage;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FelisBroker.Services.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddFelisBroker(this IServiceCollection services, string dataPath)
    {
        services.AddSingleton<ICollectorEventChannel, CollectorEventChannel>();

        var json = File.ReadAllText("origins.json");
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new OriginConfigurationConverter(), new HttpAuthConfigurationConverter() }
        };

        var config = JsonSerializer.Deserialize<AppConfig>(json, options)!;
        services.AddSingleton(config);

        foreach (var origin in config.Origins)
        {
            var validation = origin.Validate();
            if (!validation.Success)
            {
                throw new ApplicationException($"Validation failed for origin '{origin.Name}':\n" +
                                               string.Join("\n", validation.Errors));
            }

            services.AddSingleton<IDataSource>(sp =>
            {
                var channel = sp.GetRequiredService<ICollectorEventChannel>();
                return DataSourceFactory.Create(origin, channel);
            });
        }

        services.AddSingleton<IStorage, StorageService>(serviceProvider =>
            new StorageService(dataPath, serviceProvider.GetRequiredService<ILogger<StorageService>>()));
        services.AddHostedService<DataSourceService>();
    }

    public static void MapFelisBrokerEndpoints(this IApplicationBuilder app)
    {
        app.UseMiddleware<ErrorMiddleware>();

        app.UseRouting();

        app.UseCertificateForwarding();

        var httpSources = app.ApplicationServices.GetServices<IDataSource>()
            .OfType<IHttpEndpointSource>().ToList();

        app.UseEndpoints(endpoints =>
        {
            if (httpSources.Count != 0)
            {
                foreach (var httpSource in httpSources)
                {
                    httpSource.MapEndpoints(endpoints);
                }
            }

            endpoints.MapGet("/", () => "Felis.Broker is up and running!");
        });
    }
}