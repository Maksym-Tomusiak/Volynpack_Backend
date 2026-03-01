using Domain.Subscriptions;

namespace Application.Common.Interfaces.Repositories;

public interface ISubscriptionRepository
{
    Task<Subscription> Add(Subscription subscription, CancellationToken cancellationToken);
    Task<Subscription> Delete(Subscription subscription, CancellationToken cancellationToken);
}