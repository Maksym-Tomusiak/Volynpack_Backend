using Api.Dtos;
using Api.Modules.Errors;
using Application.NewsCategories.Commands;
using Application.NewsCategories.Exceptions;
using Application.NewsCategories.Queries;
using Domain.NewsCategories;
using LanguageExt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wolverine;

namespace Api.Controllers;

[ApiController]
public class NewsCategoriesController(IMessageBus messageBus) : ControllerBase
{
    [HttpGet("api/news-categories")]
    public async Task<IResult> GetAll(CancellationToken cancellationToken)
    {
        var query = new GetAllNewsCategoriesQuery();
        var items = await messageBus.InvokeAsync<IReadOnlyList<NewsCategory>>(query, cancellationToken);
        return Results.Ok(items.Select(NewsCategoryDto.FromDomainModel));
    }

    [HttpGet("api/news-categories/{id:guid}")]
    public async Task<IResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetNewsCategoryByIdQuery(id);
        var result = await messageBus.InvokeAsync<Either<NewsCategoryException, NewsCategory>>(query, cancellationToken);
        return result.Match<IResult>(
            category => Results.Ok(NewsCategoryDto.FromDomainModel(category)),
            ex => ex.ToIResult());
    }

    [Authorize(Roles = "Admin,HeadManager")]
    [HttpPost("api/news-categories")]
    public async Task<IResult> Create([FromBody] NewsCategoryCreateDto request, CancellationToken cancellationToken)
    {
        var cmd = new CreateNewsCategoryCommand(request.TitleUk, request.TitleEn);
        var result = await messageBus.InvokeAsync<Either<NewsCategoryException, NewsCategory>>(cmd, cancellationToken);
        return result.Match<IResult>(
            category => Results.Created($"/api/news-categories/{category.Id.Value}", NewsCategoryDto.FromDomainModel(category)),
            ex => ex.ToIResult());
    }

    [Authorize(Roles = "Admin,HeadManager")]
    [HttpPut("api/news-categories/{id:guid}")]
    public async Task<IResult> Update(Guid id, [FromBody] NewsCategoryUpdateDto request, CancellationToken cancellationToken)
    {
        var cmd = new UpdateNewsCategoryCommand(id, request.TitleUk, request.TitleEn);
        var result = await messageBus.InvokeAsync<Either<NewsCategoryException, NewsCategory>>(cmd, cancellationToken);
        return result.Match<IResult>(
            category => Results.Ok(NewsCategoryDto.FromDomainModel(category)),
            ex => ex.ToIResult());
    }

    [Authorize(Roles = "Admin,HeadManager")]
    [HttpDelete("api/news-categories/{id:guid}")]
    public async Task<IResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var cmd = new DeleteNewsCategoryCommand(id);
        var result = await messageBus.InvokeAsync<Either<NewsCategoryException, NewsCategory>>(cmd, cancellationToken);
        return result.Match<IResult>(
            category => Results.Ok(NewsCategoryDto.FromDomainModel(category)),
            ex => ex.ToIResult());
    }
}

