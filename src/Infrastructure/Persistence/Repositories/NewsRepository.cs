using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Common.Models;
using Domain.News;
using LanguageExt;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class NewsRepository(ApplicationDbContext context) : INewsRepository, INewsQueries
{
    public async Task<News> Add(News news, CancellationToken cancellationToken)
    {
        await context.News.AddAsync(news, cancellationToken);

        // AddAsync marks the entire graph as Added.
        // Hashtags already exist in the DB, so reset their state to Unchanged
        // to prevent EF from trying to INSERT them again.
        foreach (var hashtag in news.Hashtags)
        {
            context.Entry(hashtag).State = EntityState.Unchanged;
        }

        await context.SaveChangesAsync(cancellationToken);
        return news;
    }

    public async Task<News> Update(News news, CancellationToken cancellationToken)
    {
        // Load the existing tracked entity with its current hashtags
        var tracked = await context.News
            .Include(x => x.Hashtags)
            .FirstAsync(x => x.Id == news.Id, cancellationToken);

        // Apply scalar / owned property changes
        context.Entry(tracked).CurrentValues.SetValues(news);
        tracked.Title = news.Title;
        tracked.SeoUrl = news.SeoUrl;
        tracked.Preface = news.Preface;
        tracked.Afterword = news.Afterword;
        tracked.CtaButtonText = news.CtaButtonText;
        tracked.CtaButtonLink = news.CtaButtonLink;
        tracked.PhotoUrl = news.PhotoUrl;
        tracked.IsImportant = news.IsImportant;
        tracked.CategoryId = news.CategoryId;
        tracked.UpdatedAt = news.UpdatedAt;

        // Sync many-to-many: clear old links and add new ones
        tracked.Hashtags.Clear();
        foreach (var hashtag in news.Hashtags)
        {
            var existing = await context.Hashtags.FindAsync([hashtag.Id], cancellationToken);
            if (existing is not null)
                tracked.Hashtags.Add(existing);
        }

        await context.SaveChangesAsync(cancellationToken);
        return tracked;
    }

    public async Task<News> Delete(News news, CancellationToken cancellationToken)
    {
        context.News.Remove(news);
        await context.SaveChangesAsync(cancellationToken);
        return news;
    }

    public async Task<Option<News>> GetById(NewsId id, CancellationToken cancellationToken)
    {
        var entity = await context.News
            .AsNoTracking()
            .Include(x => x.Category)
            .Include(x => x.Sections.OrderBy(s => s.Order))
            .Include(x => x.Hashtags)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return entity is null ? Option<News>.None : Option<News>.Some(entity);
    }

    public async Task<PaginatedResult<News>> GetPaginated(PaginationParameters parameters, CancellationToken cancellationToken)
    {
        var query = context.News
            .AsNoTracking()
            .Include(x => x.Category)
            .Include(x => x.Hashtags)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(parameters.SearchTerm))
        {
            var term = parameters.SearchTerm.ToLower();
            // Filter on raw JSON column text — EF Core will translate via LIKE
            query = query.Where(x =>
                EF.Functions.ILike(x.Title.Uk, $"%{term}%") ||
                EF.Functions.ILike(x.Title.En, $"%{term}%") ||
                EF.Functions.ILike(x.Preface.Uk, $"%{term}%") ||
                EF.Functions.ILike(x.Preface.En, $"%{term}%"));
        }

        if (!string.IsNullOrWhiteSpace(parameters.SortBy))
        {
            query = parameters.SortBy.ToLower() switch
            {
                "createdat" => parameters.SortDescending
                    ? query.OrderByDescending(x => x.CreatedAt)
                    : query.OrderBy(x => x.CreatedAt),
                "updatedat" => parameters.SortDescending
                    ? query.OrderByDescending(x => x.UpdatedAt)
                    : query.OrderBy(x => x.UpdatedAt),
                _ => query.OrderByDescending(x => x.CreatedAt)
            };
        }
        else
        {
            query = query.OrderByDescending(x => x.CreatedAt);
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var totalPages = (int)Math.Ceiling(totalCount / (double)parameters.PageSize);

        var items = await query
            .Skip((parameters.PageNumber - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedResult<News>(items, totalCount, parameters.PageNumber, parameters.PageSize, totalPages);
    }

    public async Task<IReadOnlyList<News>> GetSimilar(NewsId newsId, int count, CancellationToken cancellationToken)
    {
        // Load the source news with its hashtags
        var source = await context.News
            .AsNoTracking()
            .Include(x => x.Hashtags)
            .Include(x => x.Category)
            .FirstOrDefaultAsync(x => x.Id == newsId, cancellationToken);

        if (source is null)
            return Array.Empty<News>();

        var sourceHashtagIds = source.Hashtags.Select(h => h.Id).ToList();

        if (sourceHashtagIds.Count == 0)
        {
            // Fallback: return latest news in the same category
            return await context.News
                .AsNoTracking()
                .Include(x => x.Category)
                .Include(x => x.Hashtags)
                .Where(x => x.Id != newsId && x.CategoryId == source.CategoryId)
                .OrderByDescending(x => x.CreatedAt)
                .Take(count)
                .ToListAsync(cancellationToken);
        }

        // Rank candidates by number of shared hashtags (descending), then by date
        var candidates = await context.News
            .AsNoTracking()
            .Include(x => x.Category)
            .Include(x => x.Hashtags)
            .Where(x => x.Id != newsId && x.Hashtags.Any(h => sourceHashtagIds.Contains(h.Id)))
            .ToListAsync(cancellationToken);

        return candidates
            .OrderByDescending(n => n.Hashtags.Count(h => sourceHashtagIds.Contains(h.Id)))
            .ThenByDescending(n => n.CreatedAt)
            .Take(count)
            .ToList();
    }

    public async Task<Option<News>> GetBySeoUrl(string seoUrl, CancellationToken cancellationToken)
    {
        var entity = await context.News
            .AsNoTracking()
            .Include(x => x.Category)
            .Include(x => x.Sections.OrderBy(s => s.Order))
            .Include(x => x.Hashtags)
            .FirstOrDefaultAsync(x =>
                x.SeoUrl.Uk == seoUrl || x.SeoUrl.En == seoUrl, cancellationToken);

        return entity is null ? Option<News>.None : Option<News>.Some(entity);
    }

    public async Task<Option<News>> GetImportant(CancellationToken cancellationToken)
    {
        var entity = await context.News
            .AsNoTracking()
            .Include(x => x.Category)
            .Include(x => x.Sections.OrderBy(s => s.Order))
            .Include(x => x.Hashtags)
            .Where(x => x.IsImportant)
            .OrderByDescending(x => x.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);

        return entity is null ? Option<News>.None : Option<News>.Some(entity);
    }

    public async Task<IReadOnlyList<Domain.LocalizedString>> GetAllSeoUrls(CancellationToken cancellationToken)
    {
        return await context.News
            .AsNoTracking()
            .Where(x => x.SeoUrl != null)
            .Select(x => x.SeoUrl)
            .ToListAsync(cancellationToken);
    }
}

