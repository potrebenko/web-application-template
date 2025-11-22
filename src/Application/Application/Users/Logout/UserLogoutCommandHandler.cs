using Application.Abstractions.Authentication;
using Microsoft.IdentityModel.JsonWebTokens;

namespace Application.Users.Logout;

public class UserLogoutCommandHandler(ITokenBlacklistService tokenBlacklistService) : IRequestHandler<LogoutUserCommand, bool>
{
    public Task<bool> Handle(LogoutUserCommand request, CancellationToken cancellationToken)
    {
        // Parse the token to get its expiration time
        var token = request.Token;
        var handler = new JsonWebTokenHandler();
        var jsonToken = handler.ReadJsonWebToken(token);
                
        // Get the expiration time
        var expiryTime = DateTime.UtcNow;
        if (jsonToken.TryGetPayloadValue<long>("exp", out var exp))
        {
            // Convert Unix timestamp to DateTime
            expiryTime = DateTimeOffset.FromUnixTimeSeconds(exp).UtcDateTime;
        }
                
        // Add the token to the blacklist
        tokenBlacklistService.BlacklistToken(token, expiryTime);

        return Task.FromResult(true);
    }
}