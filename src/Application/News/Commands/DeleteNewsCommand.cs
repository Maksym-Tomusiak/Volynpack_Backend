using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Common.Interfaces.Services;
using Application.News.Exceptions;
using Domain.News;
using LanguageExt;

namespace Application.News.Commands;

public record DeleteNewsCommand(Guid Id);

public static class DeleteNewsCommandHandler
{
    public static async Task<Either<NewsException, Domain.News.News>> Handle(
        DeleteNewsCommand command,
        INewsRepository newsRepository,
        INewsQueries newsQueries,
        IFileService fileService,
        CancellationToken cancellationToken)
    {
        var newsId = new NewsId(command.Id);
        var existing = await newsQueries.GetById(newsId, cancellationToken);
        try
        {
            return await existing.Match<Task<Either<NewsException, Domain.News.News>>>(
                async news =>
                {
                    // Delete the associated image file
                    if (!string.IsNullOrEmpty(news.PhotoUrl))
                        await fileService.DeleteFileAsync(news.PhotoUrl, "news", cancellationToken);

                    return await newsRepository.Delete(news, cancellationToken);
                },
                () => Task.FromResult<Either<NewsException, Domain.News.News>>(new NewsNotFoundException(command.Id)));
        }
        catch (Exception ex)
        {
            return new NewsUnknownException(command.Id, ex);
        }
    }
}

