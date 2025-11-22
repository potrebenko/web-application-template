using Application.Abstractions.Authentication;

namespace Infrastructure.Authentication;

public class UserContext : IUserContext
{
    public Guid UserId { get; }
}