using Api.Dtos;
using Api.Dtos.PackageTypes;
using Api.Modules.Errors;
using Application.Common.Models;
using Application.PackageTypes.Commands;
using Application.PackageTypes.Exceptions;
using Application.PackageTypes.Queries;
using Domain.PackageTypes;
using LanguageExt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wolverine;

namespace Api.Controllers;

[ApiController]
public class PackageTypesController(IMessageBus messageBus) : ControllerBase
{
    [HttpGet("api/package-types")]
    public async Task<IResult> GetPaginated(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? searchTerm = null,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool sortDescending = false,
        CancellationToken cancellationToken = default)
    {
        var query = new GetPackageTypesPaginatedQuery(pageNumber, pageSize, searchTerm, sortBy, sortDescending);
        var result = await messageBus.InvokeAsync<PaginatedResult<PackageType>>(query, cancellationToken);
        return Results.Ok(PaginatedResult<PackageType>.MapFrom(result, PackageTypeDto.FromDomainModel));
    }

    [HttpGet("api/package-types/all")]
    public async Task<IResult> GetAll(CancellationToken cancellationToken)
    {
        var query = new GetAllPackageTypesQuery();
        var items = await messageBus.InvokeAsync<IReadOnlyList<PackageType>>(query, cancellationToken);
        return Results.Ok(items.Select(PackageTypeDto.FromDomainModel));
    }

    [HttpGet("api/package-types/{id:guid}")]
    public async Task<IResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetPackageTypeByIdQuery(id);
        var result = await messageBus.InvokeAsync<Either<PackageTypeException, PackageType>>(query, cancellationToken);
        return result.Match<IResult>(
            packageType => Results.Ok(PackageTypeDto.FromDomainModel(packageType)),
            ex => ex.ToIResult());
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("api/package-types")]
    public async Task<IResult> Create([FromForm] PackageTypeCreateDto request, CancellationToken cancellationToken)
    {
        var cmd = new CreatePackageTypeCommand(request.TitleUk, request.TitleEn, request.ImageIcon);
        var result = await messageBus.InvokeAsync<Either<PackageTypeException, PackageType>>(cmd, cancellationToken);
        return result.Match<IResult>(
            packageType => Results.Created($"/api/package-types/{packageType.Id.Value}", PackageTypeDto.FromDomainModel(packageType)),
            ex => ex.ToIResult());
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("api/package-types/{id:guid}")]
    public async Task<IResult> Update(Guid id, [FromForm] PackageTypeUpdateDto request, CancellationToken cancellationToken)
    {
        var cmd = new UpdatePackageTypeCommand(id, request.TitleUk, request.TitleEn, request.ImageIcon);
        var result = await messageBus.InvokeAsync<Either<PackageTypeException, PackageType>>(cmd, cancellationToken);
        return result.Match<IResult>(
            packageType => Results.Ok(PackageTypeDto.FromDomainModel(packageType)),
            ex => ex.ToIResult());
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("api/package-types/{id:guid}")]
    public async Task<IResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var cmd = new DeletePackageTypeCommand(id);
        var result = await messageBus.InvokeAsync<Either<PackageTypeException, PackageType>>(cmd, cancellationToken);
        return result.Match<IResult>(
            packageType => Results.Ok(PackageTypeDto.FromDomainModel(packageType)),
            ex => ex.ToIResult());
    }
}
