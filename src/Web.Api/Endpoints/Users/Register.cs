using Application.Users.Register;
using MediatR;

namespace Web.Api.Endpoints;

public class Register : IEndpoint
{
    public sealed record Request(string Name, string Email, string Password);


    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("users/register", async (Request request, ISender sender, CancellationToken cancellationToken) =>
        {
            var command = new RegisterUserCommand(request.Name, request.Email, request.Password);
            var response = await sender.Send(command, cancellationToken);
            return response;
        })
        .WithTags("Users")
        .WithName("Register")
        .WithOpenApi();
    }
}