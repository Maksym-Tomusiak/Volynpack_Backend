using Application.Common.Models;
using Domain.News;
using LanguageExt;

namespace Application.Common.Interfaces.Queries;

public interface INewsQueries
{
    Task<Option<Domain.News.News>> GetById(NewsId id, CancellationToken cancellationToken);
    Task<PaginatedResult<Domain.News.News>> GetPaginated(PaginationParameters parameters, CancellationToken cancellationToken);
    Task<IReadOnlyList<Domain.News.News>> GetSimilar(NewsId newsId, int count, CancellationToken cancellationToken);
    Task<Option<Domain.News.News>> GetBySeoUrl(string seoUrl, CancellationToken cancellationToken);
    Task<Option<Domain.News.News>> GetImportant(CancellationToken cancellationToken);
    Task<IReadOnlyList<Domain.LocalizedString>> GetAllSeoUrls(CancellationToken cancellationToken);
}

