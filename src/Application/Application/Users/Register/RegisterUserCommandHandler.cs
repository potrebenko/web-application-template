using Application.Abstractions.Authentication;
using Application.Abstractions.Database;
using Domain.Users;

namespace Application.Users.Register;

public class RegisterUserCommandHandler(IUserRepository userRepository, IPasswordHasher passwordHasher)
    : IRequestHandler<RegisterUserCommand, string?>
{
    public async Task<string?> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var hasUser = await userRepository.HasUserAsync(request.Email);

        if (hasUser)
        {
            return null;
        }
        
        var user = CreateUser(request, passwordHasher);
        
        await userRepository.AddUserAsync(user);

        return user.Id;
    }

    private static UserDocument CreateUser(RegisterUserCommand request, IPasswordHasher passwordHasher)
    {
        return new UserDocument
        {
            Id = Guid.NewGuid().ToString(),
            Email = request.Email,
            PasswordHash = passwordHasher.HashPassword(request.Password),
            CreatedAt = DateTime.UtcNow,
            Name = request.Name
        };
    }
}