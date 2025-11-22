using Domain.Users;

namespace Application.Users.Register;

public class UserRegisteredDomainEventHandler(ILogger<UserRegisteredDomainEventHandler> logger) : INotificationHandler<UserRegisteredDomainEvent>
{
    public Task Handle(UserRegisteredDomainEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("User {UserId} registered", notification.UserId);
        return Task.CompletedTask;
    }
}