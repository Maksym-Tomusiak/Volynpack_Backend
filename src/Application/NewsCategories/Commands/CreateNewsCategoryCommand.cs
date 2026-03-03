using Application.Common.Interfaces.Repositories;
using Application.NewsCategories.Exceptions;
using Domain.NewsCategories;
using LanguageExt;

namespace Application.NewsCategories.Commands;

public record CreateNewsCategoryCommand(string TitleUk, string TitleEn);

public static class CreateNewsCategoryCommandHandler
{
    public static async Task<Either<NewsCategoryException, NewsCategory>> Handle(
        CreateNewsCategoryCommand command,
        INewsCategoryRepository newsCategoryRepository,
        CancellationToken cancellationToken)
    {
        try
        {
            var title = new Domain.LocalizedString(command.TitleUk, command.TitleEn);
            var category = NewsCategory.New(title);
            return await newsCategoryRepository.Add(category, cancellationToken);
        }
        catch (Exception ex)
        {
            return new NewsCategoryUnknownException(Guid.Empty, ex);
        }
    }
}

