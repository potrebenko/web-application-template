using Domain.Users;

namespace Application.Abstractions.Database;

public interface IUserRepository : IRepository
{
    Task<UserDocument> GetByEmailAsync(string requestEmail);
    Task<bool> HasUserAsync(string requestEmail);
    Task AddUserAsync(UserDocument user);
}
