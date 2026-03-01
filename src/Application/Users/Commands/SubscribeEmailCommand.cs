using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Domain.Subscriptions;
using LanguageExt;

namespace Application.Users.Commands;

public record SubscribeEmailCommand(string Email);

public class SubscribeEmailCommandHandler
{
    public static async Task<Either<bool, Subscription>> Handle(
        SubscribeEmailCommand command,
        ISubscriptionQueries subscriptionQueries,
        ISubscriptionRepository subscriptionRepository,
        CancellationToken cancellationToken)
    {
        var subscription = await subscriptionQueries.GetByEmail(command.Email, cancellationToken);
        return await subscription.Match<Task<Either<bool, Subscription>>>(
            s => Task.FromResult<Either<bool, Subscription>>(false),
            async () => await subscriptionRepository.Add(Subscription.New(command.Email), cancellationToken)
        );
    }
}