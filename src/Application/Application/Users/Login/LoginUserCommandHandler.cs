using Application.Abstractions.Authentication;
using Application.Abstractions.Database;
using Domain.Users;

namespace Application.Users.Login;

public class LoginUserCommandHandler(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    ITokenProvider tokenProvider) : IRequestHandler<LoginUserCommand, (string?, string?)>
{
    public async Task<(string?, string?)> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        UserDocument? user = await userRepository.GetByEmailAsync(request.Email);

        if (user is null)
        {
            return (null, null); 
        }
        
        bool verified = passwordHasher.VerifyPassword(user.PasswordHash, request.Password);

        if (!verified)
        {
            return (null, null);
        }

        string token = tokenProvider.CreateAuthToken(user.Id, user.Email);
        string refreshToken = await tokenProvider.CreateAndSaveRefreshTokenAsync(user.Id, user.Email, request.IpAddress);
        return (token, refreshToken);
    }
}