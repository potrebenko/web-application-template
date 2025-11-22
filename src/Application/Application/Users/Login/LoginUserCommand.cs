namespace Application.Users.Login;

public sealed record LoginUserCommand(string Email, string Password, string IpAddress) : IRequest<(string?, string?)>;
