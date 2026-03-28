using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Domain.NewsCategories;
using LanguageExt;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class NewsCategoryRepository(ApplicationDbContext context) : INewsCategoryRepository, INewsCategoryQueries
{
    public async Task<NewsCategory> Add(NewsCategory category, CancellationToken cancellationToken)
    {
        await context.NewsCategories.AddAsync(category, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return category;
    }

    public async Task<NewsCategory> Update(NewsCategory category, CancellationToken cancellationToken)
    {
        context.NewsCategories.Update(category);
        await context.SaveChangesAsync(cancellationToken);
        return category;
    }

    public async Task<NewsCategory> Delete(NewsCategory category, CancellationToken cancellationToken)
    {
        context.NewsCategories.Remove(category);
        await context.SaveChangesAsync(cancellationToken);
        return category;
    }

    public async Task<IReadOnlyList<NewsCategory>> GetAll(CancellationToken cancellationToken)
    {
        return await context.NewsCategories
            .AsNoTracking()
            .OrderBy(x => x.Title.Uk)
            .ToListAsync(cancellationToken);
    }

    public async Task<Option<NewsCategory>> GetById(NewsCategoryId id, CancellationToken cancellationToken)
    {
        var entity = await context.NewsCategories
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return entity is null ? Option<NewsCategory>.None : Option<NewsCategory>.Some(entity);
    }
}

