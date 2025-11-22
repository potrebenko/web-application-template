using Domain.Security;
using Domain.Users;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace Infrastructure.Database;

public class MongoDbContext : IMongoDbContext
{
    public IMongoCollection<UserDocument> Users { get; }
    public IMongoCollection<RefreshTokenDocument> RefreshTokens { get; }
    public IMongoCollection<BlackListTokenDocument> BlackListTokens { get; }

    public MongoDbContext(IConfiguration configuration)
    {
        var mongoUrl = MongoUrl.Create(configuration.GetConnectionString("MongoDB"));
        var client = new MongoClient(mongoUrl);
        var database = client.GetDatabase(mongoUrl.DatabaseName); 
        Users = database.GetCollection<UserDocument>("users");
        RefreshTokens = database.GetCollection<RefreshTokenDocument>("refreshTokens");
        BlackListTokens = database.GetCollection<BlackListTokenDocument>("tokens");
    }
}