using Api.Dtos.ProductPhotos;
using Api.Modules.Errors;
using Application.Common.Models;
using Application.ProductPhotos.Commands;
using Application.ProductPhotos.Exceptions;
using Application.ProductPhotos.Queries;
using Domain.ProductPhotos;
using LanguageExt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Wolverine;

namespace Api.Controllers;

[EnableRateLimiting("CatalogPolicy")]
[ApiController]
public class ProductPhotoController(IMessageBus messageBus) : ControllerBase
{
    [HttpGet("api/product-photos")]
    public async Task<IResult> GetPaginated(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? searchTerm = null,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool sortDescending = false,
        CancellationToken cancellationToken = default)
    {
        var query = new GetProductPhotoPaginatedQuery(pageNumber, pageSize, searchTerm, sortBy, sortDescending);
        var result = await messageBus.InvokeAsync<PaginatedResult<ProductPhoto>>(query, cancellationToken);
        
        return Results.Ok(PaginatedResult<ProductPhoto>.MapFrom(result, ProductPhotoDto.FromDomainModel));
    }

    [HttpGet("api/product-photos/{id:guid}")]
    public async Task<IResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetProductPhotoByIdQuery(id);
        var result = await messageBus.InvokeAsync<Either<ProductPhotoException, ProductPhoto>>(query, cancellationToken);
        
        return result.Match<IResult>(
            photo => Results.Ok(ProductPhotoDto.FromDomainModel(photo)),
            ex => ex.ToIResult());
    }

    // Спеціальний ендпойнт для отримання всіх фото конкретної варіації товару
    [HttpGet("api/product-variants/{variantId:guid}/photos")]
    public async Task<IResult> GetByVariantId(Guid variantId, CancellationToken cancellationToken)
    {
        var query = new GetProductPhotosByVariantIdQuery(variantId);
        var result = await messageBus.InvokeAsync<IReadOnlyList<ProductPhoto>>(query, cancellationToken);
        
        return Results.Ok(result.Select(ProductPhotoDto.FromDomainModel));
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("api/product-photos")]
    public async Task<IResult> Create([FromForm] ProductPhotoCreateDto request, CancellationToken cancellationToken)
    {
        var cmd = new CreateProductPhotoCommand(request.ProductVariantId, request.Photo, request.IsPrimary);
        var result = await messageBus.InvokeAsync<Either<ProductPhotoException, ProductPhoto>>(cmd, cancellationToken);
        
        return result.Match<IResult>(
            photo => Results.Created($"/api/product-photos/{photo.Id.Value}", ProductPhotoDto.FromDomainModel(photo)),
            ex => ex.ToIResult());
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("api/product-photos/{id:guid}")]
    public async Task<IResult> Update(Guid id, [FromBody] ProductPhotoUpdateDto request, CancellationToken cancellationToken)
    {
        var cmd = new UpdateProductPhotoCommand(id, request.IsPrimary);
        var result = await messageBus.InvokeAsync<Either<ProductPhotoException, ProductPhoto>>(cmd, cancellationToken);
        
        return result.Match<IResult>(
            photo => Results.Ok(ProductPhotoDto.FromDomainModel(photo)),
            ex => ex.ToIResult());
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("api/product-photos/{id:guid}")]
    public async Task<IResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var cmd = new DeleteProductPhotoCommand(id);
        var result = await messageBus.InvokeAsync<Either<ProductPhotoException, ProductPhoto>>(cmd, cancellationToken);
        
        return result.Match<IResult>(
            photo => Results.Ok(ProductPhotoDto.FromDomainModel(photo)),
            ex => ex.ToIResult());
    }
}