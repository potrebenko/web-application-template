using Application.Users.Login;
using Infrastructure;
using Infrastructure.Extensions;
using MediatR;

namespace Web.Api.Endpoints;

public class Login : IEndpoint
{
    private sealed record Request(string Email, string Password);
    
    private sealed record Response(string Token);
    
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("users/login", async (HttpContext context, Request request, ISender sender, 
                CancellationToken cancellationToken) =>
        {
            var ipAddress = context.GetIpAddress();
            var command = new LoginUserCommand(request.Email, request.Password, ipAddress);
            var (token, refreshToken) = await sender.Send(command, cancellationToken);
            var response = Results.Ok(new Response(token));
            return response.WithCookie("refreshToken", refreshToken, new CookieOptions
            {
                HttpOnly = false,
                Secure = false,
                SameSite = SameSiteMode.Lax,
                Path = "/",
                Expires = DateTime.UtcNow.AddDays(7)
            });
        })
        .WithTags("Users")
        .WithName("Login")
        .WithOpenApi();
    }
}