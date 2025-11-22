namespace Application.Users;

public sealed record GetByIdQuery(string Id) : IRequest<string>;