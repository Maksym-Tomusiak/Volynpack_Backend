using Application.Common.Interfaces.Queries;
using Domain.OrderStatuses;
using LanguageExt;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class OrderStatusRepository(ApplicationDbContext context) : IOrderStatusQueries
{
    public async Task<IReadOnlyList<OrderStatus>> GetAll(CancellationToken cancellationToken)
    {
        return await context.OrderStatuses
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<Option<OrderStatus>> GetByName(string name, CancellationToken cancellationToken)
    {
        var entity = await context.OrderStatuses
            .FirstOrDefaultAsync(x => x.Name == name, cancellationToken);
        
        return entity is null ? Option<OrderStatus>.None : Option<OrderStatus>.Some(entity);
    }
}