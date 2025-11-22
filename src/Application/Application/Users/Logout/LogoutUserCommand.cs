using MediatR;

namespace Application.Users.Logout;

public sealed record LogoutUserCommand(string Token) : IRequest<bool>;