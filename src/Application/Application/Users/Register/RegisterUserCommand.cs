namespace Application.Users.Register;

public sealed record RegisterUserCommand(string Name, string Email, string Password)
    : IRequest<string?>;