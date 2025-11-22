using Domain.Security;

namespace Application.Abstractions.Database;

public interface IRefreshTokenRepository : IRepository
{
    Task AddTokenAsync(RefreshTokenDocument token);
    Task<RefreshTokenDocument> GetTokenAsync(string token);
    Task DeleteTokenAsync(string refreshToken);
}