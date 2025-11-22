using Application.Users.Token;
using MediatR;

namespace Web.Api.Endpoints;

public class RefreshToken : IEndpoint
{
    public sealed record Response(string Token);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("users/refresh-token",
            async (HttpContext httpContext, ISender sender, CancellationToken cancellationToken) =>
            {
                var refreshToken = httpContext.Request.Cookies["refreshToken"];
                var command = new ValidateRefreshTokenCommand(refreshToken);
                var response = await sender.Send(command, cancellationToken);
                return new Response(response);
            })
            .WithTags("Users")
            .WithName("RefreshToken")
            .WithOpenApi();

    }
}