using System.Security.Claims;
using System.Security.Cryptography;
using Application.Abstractions.Authentication;
using Application.Abstractions.Database;
using Domain.Security;
using Infrastructure.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Authentication;

public class TokenProvider(IOptions<JwtConfiguration> jwtOptions, IRefreshTokenRepository refreshTokenRepository) : ITokenProvider
{
    public string CreateAuthToken(string userId, string email)
    {
        var jwtConfig = jwtOptions.Value;
        var secretKey = jwtConfig.Secret;
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
            [
                new Claim(JwtRegisteredClaimNames.Sub, userId),
                new Claim(JwtRegisteredClaimNames.Email, email)
            ]),
            Expires = DateTime.UtcNow.AddMinutes(jwtConfig.ExpirationInMinutes),
            SigningCredentials = credentials,
            Issuer = jwtConfig.Issuer,
            Audience = jwtConfig.Audience
        };

        var handler = new JsonWebTokenHandler();

        var token = handler.CreateToken(tokenDescriptor);

        return token;
    }

    public string CreateRefreshToken(int length = 32)
    {
        byte[] randomBytes = new byte[length];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes).Replace('+', '-').Replace('/', '_').Replace("=", "");
    }

    public async ValueTask<(string?, bool)> ValidateRefreshTokenAsync(string requestRefreshToken)
    {
        var hashedToken = HashToken(requestRefreshToken);
        var refreshToken = await refreshTokenRepository.GetTokenAsync(hashedToken);
        if (refreshToken is null)
        {
            return (null, false);
        }

        if (refreshToken.ExpiryTime > DateTime.UtcNow)
        {
            var token = CreateAuthToken(refreshToken.UserId, refreshToken.Email);
            return (token, true);
        }

        return (null, false);
    }

    public Task RevokeRefreshTokenAsync(string refreshToken)
    {
        return refreshTokenRepository.DeleteTokenAsync(refreshToken);
    }

    public async Task<string> CreateAndSaveRefreshTokenAsync(string userId, string email, string ipAddress)
    {
        var expirationInMinutes = jwtOptions.Value.RefreshTokenExpirationInMinutes;
        var expiryTime = DateTime.UtcNow.AddMinutes(expirationInMinutes);
        var refreshToken = CreateRefreshToken();
        var hashedToken = HashToken(refreshToken);
        var tokenDocument = CreateTokenDocument(hashedToken, userId, email, ipAddress, expiryTime);
        await refreshTokenRepository.AddTokenAsync(tokenDocument);
        return refreshToken;
    }

    private string HashToken(string refreshToken)
    {
        using var sha256 = SHA256.Create();
        var buffer = Encoding.UTF8.GetBytes(refreshToken);
        var hashedBytes = sha256.ComputeHash(buffer);
        return Convert.ToBase64String(hashedBytes);
    }

    private static RefreshTokenDocument CreateTokenDocument(string token, string userId, string email, string ipAddress,
        DateTime expiryTime)
    {
        return new RefreshTokenDocument
        {
            Token = token,
            UserId = userId,
            Email = email,
            IpAddress = ipAddress,
            ExpiryTime = expiryTime,
            CreatedAt = DateTime.UtcNow
        };
    }
}