using Application.Abstractions.Database;
using Domain.Security;
using MongoDB.Driver;

namespace Infrastructure.Database;

public class RefreshTokenRepository(IMongoDbContext dbContext) : IRefreshTokenRepository
{
    private readonly IMongoCollection<RefreshTokenDocument> _tokenCollection = dbContext.RefreshTokens;

    public Task AddTokenAsync(RefreshTokenDocument token)
    {
        return _tokenCollection.InsertOneAsync(token);
    }
    

    public Task<RefreshTokenDocument> GetTokenAsync(string token)
    {
        return _tokenCollection.Find(x => x.Token == token).FirstOrDefaultAsync();
    }

    public Task DeleteTokenAsync(string refreshToken)
    {
        return _tokenCollection.DeleteOneAsync(refreshToken);
    }
}