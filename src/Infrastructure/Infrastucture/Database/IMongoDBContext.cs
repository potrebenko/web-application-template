using Domain.Security;
using Domain.Users;
using MongoDB.Driver;

namespace Infrastructure.Database;

public interface IMongoDbContext
{
    IMongoCollection<UserDocument> Users { get; }
    IMongoCollection<RefreshTokenDocument> RefreshTokens { get; }
    
    IMongoCollection<BlackListTokenDocument> BlackListTokens { get; }
}