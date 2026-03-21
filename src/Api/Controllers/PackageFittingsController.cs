using Api.Dtos.PackageFittings;
using Api.Modules.Errors;
using Application.Common.Models;
using Application.PackageFittings.Commands;
using Application.PackageFittings.Exceptions;
using Application.PackageFittings.Queries;
using Domain.PackageFittings;
using LanguageExt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Wolverine;

namespace Api.Controllers;

[EnableRateLimiting("CatalogPolicy")]
[ApiController]
public class PackageFittingsController(IMessageBus messageBus) : ControllerBase
{
    [HttpGet("api/package-fittings")]
    public async Task<IResult> GetPaginated(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? searchTerm = null,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool sortDescending = false,
        CancellationToken cancellationToken = default)
    {
        var query = new GetPackageFittingsPaginatedQuery(pageNumber, pageSize, searchTerm, sortBy, sortDescending);
        var result = await messageBus.InvokeAsync<PaginatedResult<PackageFitting>>(query, cancellationToken);
        return Results.Ok(PaginatedResult<PackageFitting>.MapFrom(result, PackageFittingDto.FromDomainModel));
    }

    [HttpGet("api/package-fittings/all")]
    public async Task<IResult> GetAll(CancellationToken cancellationToken)
    {
        var query = new GetAllPackageFittingsQuery();
        var items = await messageBus.InvokeAsync<IReadOnlyList<PackageFitting>>(query, cancellationToken);
        return Results.Ok(items.Select(PackageFittingDto.FromDomainModel));
    }

    [HttpGet("api/package-fittings/{id:guid}")]
    public async Task<IResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetPackageFittingByIdQuery(id);
        var result = await messageBus.InvokeAsync<Either<PackageFittingException, PackageFitting>>(query, cancellationToken);
        return result.Match<IResult>(
            fitting => Results.Ok(PackageFittingDto.FromDomainModel(fitting)),
            ex => ex.ToIResult());
    }
    
    [HttpGet("api/package-fittings/type/{typeId:guid}/material/{materialId:guid}")]
    public async Task<IResult> GetByTypeAndMaterial(Guid typeId, Guid materialId, CancellationToken cancellationToken)
    {
        var query = new GetPackageFittingByTypeAndMaterialQuery(typeId, materialId);
        var result = await messageBus.InvokeAsync<Either<PackageFittingException, PackageFitting>>(query, cancellationToken);
        return result.Match<IResult>(
            fitting => Results.Ok(PackageFittingDto.FromDomainModel(fitting)),
            ex => ex.ToIResult());
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("api/package-fittings")]
    public async Task<IResult> Create([FromForm] PackageFittingCreateDto request, CancellationToken cancellationToken)
    {
        var cmd = new CreatePackageFittingCommand(request.TypeId, request.MaterialId, request.FittingImage);
        var result = await messageBus.InvokeAsync<Either<PackageFittingException, PackageFitting>>(cmd, cancellationToken);
        return result.Match<IResult>(
            fitting => Results.Created($"/api/package-fittings/{fitting.Id.Value}", PackageFittingDto.FromDomainModel(fitting)),
            ex => ex.ToIResult());
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("api/package-fittings/{id:guid}")]
    public async Task<IResult> Update(Guid id, [FromForm] PackageFittingUpdateDto request, CancellationToken cancellationToken)
    {
        var cmd = new UpdatePackageFittingCommand(id, request.TypeId, request.MaterialId, request.FittingImage);
        var result = await messageBus.InvokeAsync<Either<PackageFittingException, PackageFitting>>(cmd, cancellationToken);
        return result.Match<IResult>(
            fitting => Results.Ok(PackageFittingDto.FromDomainModel(fitting)),
            ex => ex.ToIResult());
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("api/package-fittings/{id:guid}")]
    public async Task<IResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var cmd = new DeletePackageFittingCommand(id);
        var result = await messageBus.InvokeAsync<Either<PackageFittingException, PackageFitting>>(cmd, cancellationToken);
        return result.Match<IResult>(
            fitting => Results.Ok(PackageFittingDto.FromDomainModel(fitting)),
            ex => ex.ToIResult());
    }
}
