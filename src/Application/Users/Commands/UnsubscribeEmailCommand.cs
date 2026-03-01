using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Domain.Subscriptions;
using LanguageExt;

namespace Application.Users.Commands;

public record UnsubscribeEmailCommand(Guid Token);

public class UnsubscribeEmailCommandHandler
{
    public static async Task<Either<bool, Subscription>> Handle(
        UnsubscribeEmailCommand command,
        ISubscriptionQueries subscriptionQueries,
        ISubscriptionRepository subscriptionRepository,
        CancellationToken cancellationToken)
    {
        var subscription = await subscriptionQueries.GetByToken(command.Token, cancellationToken);
        return await subscription.Match<Task<Either<bool, Subscription>>>(
            async s =>
            {
                await subscriptionRepository.Delete(s, cancellationToken);
                return s;
            },
            () => Task.FromResult<Either<bool, Subscription>>(false)
        );
    }
}