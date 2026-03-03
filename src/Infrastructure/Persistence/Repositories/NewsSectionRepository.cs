using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Domain.News;
using Domain.NewsSections;
using LanguageExt;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class NewsSectionRepository(ApplicationDbContext context) : INewsSectionRepository, INewsSectionQueries
{
    public async Task<NewsSection> Add(NewsSection section, CancellationToken cancellationToken)
    {
        await context.NewsSections.AddAsync(section, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return section;
    }

    public async Task<NewsSection> Update(NewsSection section, CancellationToken cancellationToken)
    {
        context.NewsSections.Update(section);
        await context.SaveChangesAsync(cancellationToken);
        return section;
    }

    public async Task<NewsSection> Delete(NewsSection section, CancellationToken cancellationToken)
    {
        context.NewsSections.Remove(section);
        await context.SaveChangesAsync(cancellationToken);
        return section;
    }

    public async Task DeleteAllByNewsId(NewsId newsId, CancellationToken cancellationToken)
    {
        var sections = await context.NewsSections
            .Where(x => x.NewsId == newsId)
            .ToListAsync(cancellationToken);

        context.NewsSections.RemoveRange(sections);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<NewsSection>> GetByNewsId(NewsId newsId, CancellationToken cancellationToken)
    {
        return await context.NewsSections
            .AsNoTracking()
            .Where(x => x.NewsId == newsId)
            .OrderBy(x => x.Order)
            .ToListAsync(cancellationToken);
    }

    public async Task<Option<NewsSection>> GetById(NewsSectionId id, CancellationToken cancellationToken)
    {
        var entity = await context.NewsSections
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return entity is null ? Option<NewsSection>.None : Option<NewsSection>.Some(entity);
    }
}

