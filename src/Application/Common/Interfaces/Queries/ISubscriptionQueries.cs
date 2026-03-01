using Domain.Subscriptions;
using LanguageExt;

namespace Application.Common.Interfaces.Queries;

public interface ISubscriptionQueries
{
    Task<IReadOnlyList<Subscription>> GetAll(Guid userId, CancellationToken cancellationToken);
    Task<Option<Subscription>> GetByToken(Guid token, CancellationToken cancellationToken);
    Task<Option<Subscription>> GetByEmail(string email, CancellationToken cancellationToken);
}