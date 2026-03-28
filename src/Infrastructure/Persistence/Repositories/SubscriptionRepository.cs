using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Domain.Subscriptions;
using LanguageExt;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class SubscriptionRepository(ApplicationDbContext context) : ISubscriptionRepository, ISubscriptionQueries
{
    public async Task<Subscription> Add(Subscription subscription, CancellationToken cancellationToken)
    {
        await context.Subscriptions.AddAsync(subscription, cancellationToken);
        
        await context.SaveChangesAsync(cancellationToken);
        
        return subscription;
    }
    
    public async Task<Subscription> Delete(Subscription subscription, CancellationToken cancellationToken)
    {
        context.Subscriptions.Remove(subscription);
        
        await context.SaveChangesAsync(cancellationToken);

        return subscription;
    }

    public async Task<IReadOnlyList<Subscription>> GetAll(CancellationToken cancellationToken)
    {
        return await context.Subscriptions
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<Option<Subscription>> GetByToken(Guid token, CancellationToken cancellationToken)
    {
        var entity = await context.Subscriptions
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.UnsubscribeToken == token, cancellationToken);
        
        return entity == null ? Option<Subscription>.None: Option<Subscription>.Some(entity);
    }

    public async Task<Option<Subscription>> GetByEmail(string email, CancellationToken cancellationToken)
    {
        var entity = await context.Subscriptions
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Email == email, cancellationToken);
        
        return entity == null ? Option<Subscription>.None: Option<Subscription>.Some(entity);
    }
}