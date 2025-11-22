namespace Application.Users.Token;

public sealed record ValidateRefreshTokenCommand(string RefreshToken) : IRequest<string?>;