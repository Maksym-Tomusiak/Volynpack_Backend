using Application.Common.Interfaces.Queries;
using Domain.DelivaryMethods;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class DeliveryMethodRepository(ApplicationDbContext context) : IDeliveryMethodQueries
{
    public async Task<IReadOnlyList<DeliveryMethod>> GetAll(CancellationToken cancellationToken)
    {
        return await context.Set<DeliveryMethod>()
            .AsNoTracking()
            .OrderBy(x => x.Name.Uk)
            .ToListAsync(cancellationToken);
    }
}
