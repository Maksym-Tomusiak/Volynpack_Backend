using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.NewsCategories.Exceptions;
using Domain.NewsCategories;
using LanguageExt;

namespace Application.NewsCategories.Commands;

public record UpdateNewsCategoryCommand(Guid Id, string TitleUk, string TitleEn);

public static class UpdateNewsCategoryCommandHandler
{
    public static async Task<Either<NewsCategoryException, NewsCategory>> Handle(
        UpdateNewsCategoryCommand command,
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
            var title = new Domain.LocalizedString(command.TitleUk, command.TitleEn);
            category.Update(title);
            return await newsCategoryRepository.Update(category, cancellationToken);
        }
        catch (Exception ex)
        {
            return new NewsCategoryUnknownException(command.Id, ex);
        }
    }
}

