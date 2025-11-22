using Application.Abstractions.Database;
using Domain.Users;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Infrastructure.Database;

public class UserRepository : IUserRepository
{
    private readonly ILogger<UserRepository> _logger;
    private readonly IMongoCollection<UserDocument> _userCollection;

    public UserRepository(IMongoDbContext dbContext, ILogger<UserRepository> logger)
    {
        _logger = logger;
        _userCollection = dbContext.Users;
        _logger.LogInformation("User repository created");
    }
  
    public Task<UserDocument> GetByEmailAsync(string requestEmail)
    {
        return _userCollection.Find(x => x.Email == requestEmail).FirstOrDefaultAsync();
    }

    public async Task<bool> HasUserAsync(string requestEmail)
    {
        return await _userCollection.CountDocumentsAsync(x=> x.Email == requestEmail) > 0;
    }

    public Task AddUserAsync(UserDocument user)
    {
        return _userCollection.InsertOneAsync(user);
    }
}