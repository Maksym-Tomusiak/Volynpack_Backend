using Api.Dtos.ProductVariants;
using Api.Modules.Errors;
using Application.Common.Models;
using Application.ProductVariants.Commands;
using Application.ProductVariants.Exceptions;
using Application.ProductVariants.Queries;
using Domain.ProductVariants;
using JasperFx.Core;
using LanguageExt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wolverine;

namespace Api.Controllers;

[ApiController]
public class ProductVariantController(IMessageBus messageBus) : ControllerBase
{
    // ── Queries (Читання) ────────────────────────────────────────────────────

    [HttpGet("api/product-variants")]
    public async Task<IResult> GetPaginated(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? searchTerm = null,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool sortDescending = false,
        CancellationToken cancellationToken = default)
    {
        var query = new GetProductVariantsPaginatedQuery(pageNumber, pageSize, searchTerm, sortBy, sortDescending);
        var result = await messageBus.InvokeAsync<PaginatedResult<ProductVariant>>(query, cancellationToken);
        
        return Results.Ok(PaginatedResult<ProductVariant>.MapFrom(result, ProductVariantDto.FromDomainModel));
    }

    [HttpGet("api/product-variants/{id:guid}")]
    public async Task<IResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetProductVariantByIdQuery(id);
        var result = await messageBus.InvokeAsync<Either<ProductVariantException, ProductVariant>>(query, cancellationToken);
        
        return result.Match<IResult>(
            variant => Results.Ok(ProductVariantDto.FromDomainModel(variant)),
            ex => ex.ToIResult());
    }
    [HttpGet("api/product-variants/by-seo-url/{seoUrl}")]
    public async Task<IResult> GetBySeoUrl(string seoUrl, CancellationToken cancellationToken)
    {
        var query = new GetProductVariantBySeoUrlQuery(seoUrl);
        var result = await messageBus.InvokeAsync<Either<ProductVariantException, ProductVariant>>(query, cancellationToken);
        
        return result.Match<IResult>(
            variant => Results.Ok(ProductVariantDto.FromDomainModel(variant)),
            ex => ex.ToIResult());
    }

    // Отримати всі варіації для конкретного базового товару (для кнопок вибору розміру справа від фото)
    [HttpGet("api/products/{productId:guid}/variants")]
    public async Task<IResult> GetByProductId(Guid productId, CancellationToken cancellationToken)
    {
        var query = new GetProductVariantsByProductIdQuery(productId);
        var result = await messageBus.InvokeAsync<IReadOnlyList<ProductVariant>>(query, cancellationToken);
        
        return Results.Ok(result.Select(ProductVariantDto.FromDomainModel));
    }

    // Популярні товари (для слайдера на головній сторінці)
    [HttpGet("api/product-variants/popular")]
    public async Task<IResult> GetPopular(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new GetPopularProductVariantsPaginatedQuery(pageNumber, pageSize);
        var result = await messageBus.InvokeAsync<PaginatedResult<ProductVariant>>(query, cancellationToken);
        
        return Results.Ok(PaginatedResult<ProductVariant>.MapFrom(result, ProductVariantDto.FromDomainModel));
    }

    // Схожі товари (на основі збігів категорій базового товару)
    [HttpGet("api/products/{productId:guid}/related")]
    public async Task<IResult> GetRelated(Guid productId, [FromQuery] int limit = 4, CancellationToken cancellationToken = default)
    {
        var query = new GetRelatedProductVariantsQuery(productId, limit);
        var result = await messageBus.InvokeAsync<IReadOnlyList<ProductVariant>>(query, cancellationToken);
        
        return Results.Ok(result.Select(ProductVariantDto.FromDomainModel));
    }

    // Головний метод каталогу (Фасетний пошук)
    [HttpGet("api/product-variants/catalog")]
    public async Task<IResult> GetFilteredCatalog(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 12,
        [FromQuery] List<Guid>? categoryIds = null,
        [FromQuery] List<Guid>? typeIds = null,
        [FromQuery] List<Guid>? materialIds = null,
        [FromQuery] decimal? height = null,
        [FromQuery] decimal? width = null,
        [FromQuery] decimal? depth = null,
        [FromQuery] int? density = null,
        [FromQuery] decimal? minPrice = null,
        [FromQuery] decimal? maxPrice = null,
        [FromQuery] ProductSortOption sortBy = ProductSortOption.PriceAsc,
        CancellationToken cancellationToken = default)
    {
        var parameters = new ProductFilterParameters(
            pageNumber, pageSize, categoryIds, typeIds, materialIds, 
            height, width, depth, density, minPrice, maxPrice, sortBy);

        var query = new GetFilteredCatalogQuery(parameters);
        var result = await messageBus.InvokeAsync<CatalogFilterResult>(query, cancellationToken);

        var responseDto = new CatalogFilterResultDto(
            PaginatedResult<ProductVariant>.MapFrom(result.Products, ProductVariantDto.FromDomainModel),
            CatalogFacetsDto.FromDomainModel(result.Facets)
        );

        return Results.Ok(responseDto);
    }

    // ── Commands (Запис) ─────────────────────────────────────────────────────

    [Authorize(Roles = "Admin")]
    [HttpPost("api/product-variants")]
    public async Task<IResult> Create([FromBody] ProductVariantCreateDto request, CancellationToken cancellationToken)
    {
        var cmd = new CreateProductVariantCommand(
            request.ProductId,request.PackageMaterialId,
            request.Density, request.LoadCapacity, request.SeoUrlUk, request.SeoUrlEn,
            request.AvailabilityUk, request.AvailabilityEn,
        request.Height, request.Width, request.Depth, request.PricePerPiece, 
            request.QuantityPerPackage, request.IsPopular);

        var result = await messageBus.InvokeAsync<Either<ProductVariantException, ProductVariant>>(cmd, cancellationToken);
        
        return result.Match<IResult>(
            variant => Results.Created($"/api/product-variants/{variant.Id.Value}", ProductVariantDto.FromDomainModel(variant)),
            ex => ex.ToIResult());
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("api/product-variants/{id:guid}")]
    public async Task<IResult> Update(Guid id, [FromBody] ProductVariantUpdateDto request, CancellationToken cancellationToken)
    {
        var cmd = new UpdateProductVariantCommand(
            id, request.PackageMaterialId,
            request.Density, request.LoadCapacity, request.SeoUrlUk, request.SeoUrlEn,
            request.AvailabilityUk, request.AvailabilityEn,
            request.Height, request.Width, request.Depth, request.PricePerPiece, 
            request.QuantityPerPackage, request.IsPopular);

        var result = await messageBus.InvokeAsync<Either<ProductVariantException, ProductVariant>>(cmd, cancellationToken);
        
        return result.Match<IResult>(
            variant => Results.Ok(ProductVariantDto.FromDomainModel(variant)),
            ex => ex.ToIResult());
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("api/product-variants/{id:guid}")]
    public async Task<IResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var cmd = new DeleteProductVariantCommand(id);
        var result = await messageBus.InvokeAsync<Either<ProductVariantException, ProductVariant>>(cmd, cancellationToken);
        
        return result.Match<IResult>(
            variant => Results.Ok(ProductVariantDto.FromDomainModel(variant)),
            ex => ex.ToIResult());
    }
}