using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.NewsCategories.Exceptions;
using Domain.NewsCategories;
using LanguageExt;

namespace Application.NewsCategories.Commands;

public record DeleteNewsCategoryCommand(Guid Id);

public static class DeleteNewsCategoryCommandHandler
{
    public static async Task<Either<NewsCategoryException, NewsCategory>> Handle(
        DeleteNewsCategoryCommand command,
        INewsCategoryRepository newsCategoryRepository,
        INewsCategoryQueries newsCategoryQueries,
        CancellationToken cancellationToken)
    {
        var categoryId = new NewsCategoryId(command.Id);
        var existing = await newsCategoryQueries.GetById(categoryId, cancellationToken);
        if (existing.IsNone)
            return new NewsCategoryNotFoundException(command.Id);

        try
        {
            var category = existing.IfNoneUnsafe((NewsCategory)null!)!;
            return await newsCategoryRepository.Delete(category, cancellationToken);
        }
        catch (Exception ex)
        {
            return new NewsCategoryUnknownException(command.Id, ex);
        }
    }
}

