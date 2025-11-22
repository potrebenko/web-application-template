namespace Application.Abstractions.Authentication;

public interface ITokenProvider
{
    string CreateAuthToken(string userId, string email);
    ValueTask<(string?, bool)> ValidateRefreshTokenAsync(string refreshToken);
    Task RevokeRefreshTokenAsync(string refreshToken);
    Task<string> CreateAndSaveRefreshTokenAsync(string userId, string email, string ipAddress);
}