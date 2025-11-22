using Domain.Security;

namespace Application.Abstractions.Database;

public interface ITokenBlackListRepository : IRepository
{
    Task<List<BlackListTokenDocument>> GetAllTokensAsync();
    
    Task SaveAllTokensAsync(List<BlackListTokenDocument> tokens);
}