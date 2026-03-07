using Api.Dtos;
using Api.Dtos.PackageMaterials;
using Api.Modules.Errors;
using Application.Common.Models;
using Application.PackageMaterials.Commands;
using Application.PackageMaterials.Exceptions;
using Application.PackageMaterials.Queries;
using Domain.PackageMaterials;
using LanguageExt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wolverine;

namespace Api.Controllers;

[ApiController]
public class PackageMaterialsController(IMessageBus messageBus) : ControllerBase
{
    [HttpGet("api/package-materials")]
    public async Task<IResult> GetPaginated(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? searchTerm = null,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool sortDescending = false,
        CancellationToken cancellationToken = default)
    {
        var query = new GetPackageMaterialsPaginatedQuery(pageNumber, pageSize, searchTerm, sortBy, sortDescending);
        var result = await messageBus.InvokeAsync<PaginatedResult<PackageMaterial>>(query, cancellationToken);
        return Results.Ok(PaginatedResult<PackageMaterial>.MapFrom(result, PackageMaterialDto.FromDomainModel));
    }

    [HttpGet("api/package-materials/all")]
    public async Task<IResult> GetAll(CancellationToken cancellationToken)
    {
        var query = new GetAllPackageMaterialsQuery();
        var items = await messageBus.InvokeAsync<IReadOnlyList<PackageMaterial>>(query, cancellationToken);
        return Results.Ok(items.Select(PackageMaterialDto.FromDomainModel));
    }

    [HttpGet("api/package-materials/{id:guid}")]
    public async Task<IResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetPackageMaterialByIdQuery(id);
        var result = await messageBus.InvokeAsync<Either<PackageMaterialException, PackageMaterial>>(query, cancellationToken);
        return result.Match<IResult>(
            material => Results.Ok(PackageMaterialDto.FromDomainModel(material)),
            ex => ex.ToIResult());
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("api/package-materials")]
    public async Task<IResult> Create([FromBody] PackageMaterialCreateDto request, CancellationToken cancellationToken)
    {
        var cmd = new CreatePackageMaterialCommand(request.TitleUk, request.TitleEn);
        var result = await messageBus.InvokeAsync<Either<PackageMaterialException, PackageMaterial>>(cmd, cancellationToken);
        return result.Match<IResult>(
            material => Results.Created($"/api/package-materials/{material.Id.Value}", PackageMaterialDto.FromDomainModel(material)),
            ex => ex.ToIResult());
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("api/package-materials/{id:guid}")]
    public async Task<IResult> Update(Guid id, [FromBody] PackageMaterialUpdateDto request, CancellationToken cancellationToken)
    {
        var cmd = new UpdatePackageMaterialCommand(id, request.TitleUk, request.TitleEn);
        var result = await messageBus.InvokeAsync<Either<PackageMaterialException, PackageMaterial>>(cmd, cancellationToken);
        return result.Match<IResult>(
            material => Results.Ok(PackageMaterialDto.FromDomainModel(material)),
            ex => ex.ToIResult());
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("api/package-materials/{id:guid}")]
    public async Task<IResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var cmd = new DeletePackageMaterialCommand(id);
        var result = await messageBus.InvokeAsync<Either<PackageMaterialException, PackageMaterial>>(cmd, cancellationToken);
        return result.Match<IResult>(
            material => Results.Ok(PackageMaterialDto.FromDomainModel(material)),
            ex => ex.ToIResult());
    }
}
