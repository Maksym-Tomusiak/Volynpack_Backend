using Api.Dtos;
using Api.Modules.Errors;
using Application.Hashtags.Commands;
using Application.Hashtags.Exceptions;
using Application.Hashtags.Queries;
using Domain.Hashtags;
using LanguageExt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Wolverine;

namespace Api.Controllers;

[EnableRateLimiting("CatalogPolicy")]
[ApiController]
public class HashtagsController(IMessageBus messageBus) : ControllerBase
{
    [HttpGet("api/hashtags")]
    public async Task<IResult> GetAll(CancellationToken cancellationToken)
    {
        var query = new GetAllHashtagsQuery();
        var items = await messageBus.InvokeAsync<IReadOnlyList<Hashtag>>(query, cancellationToken);
        return Results.Ok(items.Select(HashtagDto.FromDomainModel));
    }

    [HttpGet("api/hashtags/{id:guid}")]
    public async Task<IResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetHashtagByIdQuery(id);
        var result = await messageBus.InvokeAsync<Either<HashtagException, Hashtag>>(query, cancellationToken);
        return result.Match<IResult>(
            hashtag => Results.Ok(HashtagDto.FromDomainModel(hashtag)),
            ex => ex.ToIResult());
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("api/hashtags")]
    public async Task<IResult> Create([FromBody] HashtagCreateDto request, CancellationToken cancellationToken)
    {
        var cmd = new CreateHashtagCommand(request.NameUk, request.NameEn);
        var result = await messageBus.InvokeAsync<Either<HashtagException, Hashtag>>(cmd, cancellationToken);
        return result.Match<IResult>(
            hashtag => Results.Created($"/api/hashtags/{hashtag.Id.Value}", HashtagDto.FromDomainModel(hashtag)),
            ex => ex.ToIResult());
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("api/hashtags/{id:guid}")]
    public async Task<IResult> Update(Guid id, [FromBody] HashtagUpdateDto request, CancellationToken cancellationToken)
    {
        var cmd = new UpdateHashtagCommand(id, request.NameUk, request.NameEn);
        var result = await messageBus.InvokeAsync<Either<HashtagException, Hashtag>>(cmd, cancellationToken);
        return result.Match<IResult>(
            hashtag => Results.Ok(HashtagDto.FromDomainModel(hashtag)),
            ex => ex.ToIResult());
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("api/hashtags/{id:guid}")]
    public async Task<IResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var cmd = new DeleteHashtagCommand(id);
        var result = await messageBus.InvokeAsync<Either<HashtagException, Hashtag>>(cmd, cancellationToken);
        return result.Match<IResult>(
            hashtag => Results.Ok(HashtagDto.FromDomainModel(hashtag)),
            ex => ex.ToIResult());
    }
}

