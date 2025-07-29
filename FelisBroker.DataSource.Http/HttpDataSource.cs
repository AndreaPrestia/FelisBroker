using FelisBroker.Common.Configurations;
using FelisBroker.Common.Extensions;
using FelisBroker.DataSource.Http.Helpers;
using FelisBroker.Interfaces.Channels;
using FelisBroker.Interfaces.Entities;
using FelisBroker.Interfaces.Sources;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FelisBroker.DataSource.Http;

public class HttpDataSource : IDataSource, IHttpEndpointSource
{
    private readonly HttpConfiguration _config;
    private readonly ICollectorEventChannel _channel;
    private readonly string _route;

    public HttpDataSource(HttpConfiguration config, ICollectorEventChannel channel)
    {
        _config = config;
        _channel = channel;
        _route = _config.Destination ?? throw new ApplicationException("Destination not configured");
    }

    public Task StartAsync()
    {
        return Task.CompletedTask;
    }

    public Task StopAsync()
    {
        return Task.CompletedTask;
    }

    private async Task<IResult> HandleRequestAsync(HttpContext ctx)
    {
        if (!HttpAuthHelper.IsAuthorized(ctx, _config))
            return Results.Unauthorized();

        using var reader = new StreamReader(ctx.Request.Body);
        var body = await reader.ReadToEndAsync();

        var entity = new SourceProcessingEntity
        {
            CorrelationId = Guid.NewGuid(),
            Destination = _config.Destination!,
            Payload = body,
            Origin = _config,
            Timestamp = DateTime.UtcNow.ToUnixTimestamp()
        };

        await _channel.PublishAsync(entity);
        return Results.Accepted();
    }

    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost(_route, HandleRequestAsync);
    }
}