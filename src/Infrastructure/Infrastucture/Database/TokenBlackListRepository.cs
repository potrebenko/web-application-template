using Application.Abstractions.Database;
using Domain.Security;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Infrastructure.Database;

public class TokenBlackListRepository(IMongoDbContext dbContext) : ITokenBlackListRepository
{
    private readonly IMongoCollection<BlackListTokenDocument> _tokenCollection = dbContext.BlackListTokens;
    private readonly BsonDocument _emptyDocument = new();
    public Task<List<BlackListTokenDocument>> GetAllTokensAsync()
    {
        return _tokenCollection.Find(_emptyDocument).ToListAsync();
    }

    public Task SaveAllTokensAsync(List<BlackListTokenDocument> tokens)
    {
        return _tokenCollection.InsertManyAsync(tokens);
    }
}