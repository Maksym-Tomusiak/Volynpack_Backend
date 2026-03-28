using Application.Common.Interfaces.Queries;
using Application.NewsCategories.Exceptions;
using Domain.NewsCategories;
using LanguageExt;

namespace Application.NewsCategories.Queries;

public record GetNewsCategoryByIdQuery(Guid Id);

public static class GetNewsCategoryByIdQueryHandler
{
    public static async Task<Either<NewsCategoryException, NewsCategory>> Handle(
        GetNewsCategoryByIdQuery query,
        INewsCategoryQueries newsCategoryQueries,
        CancellationToken cancellationToken)
    {
        var categoryId = new NewsCategoryId(query.Id);
        var result = await newsCategoryQueries.GetById(categoryId, cancellationToken);
        return result.Match<Either<NewsCategoryException, NewsCategory>>(
            category => category,
            () => new NewsCategoryNotFoundException(query.Id));
    }
}

