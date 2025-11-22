using Application.Abstractions.Authentication;

namespace Application.Users.Token;

public class ValidateRefreshTokenCommandHandler(ITokenProvider tokenProvider, ILogger<ValidateRefreshTokenCommandHandler> logger)
    : IRequestHandler<ValidateRefreshTokenCommand, string?>
{
    public async Task<string?> Handle(ValidateRefreshTokenCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.RefreshToken))
        {
            logger.LogWarning("Refresh token is empty");            
            return null;
        }
        
        logger.LogInformation("Validating refresh token " + request.RefreshToken);
        
        var (token, success) = await tokenProvider.ValidateRefreshTokenAsync(request.RefreshToken);
        if (success)
        {
            return token;
        }
        return null;
    }
}