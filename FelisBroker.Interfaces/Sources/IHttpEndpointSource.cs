using Microsoft.AspNetCore.Routing;

namespace FelisBroker.Interfaces.Sources;

public interface IHttpEndpointSource
{
    void MapEndpoints(IEndpointRouteBuilder app);
}