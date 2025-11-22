using Application.Users.Logout;
using MediatR;

namespace Web.Api.Endpoints;

public class Logout : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("users/logout", async (HttpContext httpContext, ISender sender) =>
            {
                // Get the token from the Authorization header
                var authHeader = httpContext.Request.Headers.Authorization.FirstOrDefault();
                if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                {
                    return Results.BadRequest(new { message = "No token provided" });
                }

                var token = authHeader.Substring("Bearer ".Length).Trim();
                var logoutUserCommand = new LogoutUserCommand(token);

                try
                {
                    var result = await sender.Send(logoutUserCommand);
                    return Results.Ok(new { message = "Logged out successfully" });
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(new { message = $"Invalid token: {ex.Message}" });
                }
            })
            .WithTags("Users")
            .WithName("Logout")
            .WithOpenApi();
    }
}