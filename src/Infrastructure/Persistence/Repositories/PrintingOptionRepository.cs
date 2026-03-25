using Application.Common.Interfaces.Queries;
using Domain.PrintingOptions;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class PrintingOptionRepository(ApplicationDbContext context) : IPrintingOptionQueries
{
    public async Task<IReadOnlyList<PrintingOption>> GetAll(CancellationToken cancellationToken)
    {
        return await context.PrintingOptions
            .AsNoTracking()
            // Сортування по англійській (або українській) назві за замовчуванням
            .OrderBy(x => x.Name.En) 
            .ToListAsync(cancellationToken);
    }
}