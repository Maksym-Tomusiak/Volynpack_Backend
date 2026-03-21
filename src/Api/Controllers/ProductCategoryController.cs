using Api.Dtos.ProductCategories;
using Api.Modules.Errors;
using Application.Common.Models;
using Application.ProductCategories.Commands;
using Application.ProductCategories.Exceptions;
using Application.ProductCategories.Queries;
using Domain.ProductCategories;
using LanguageExt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Wolverine;

namespace Api.Controllers;

[EnableRateLimiting("CatalogPolicy")]
[ApiController]
public class ProductCategoryController(IMessageBus messageBus) : ControllerBase
{
    [HttpGet("api/product-categories")]
    public async Task<IResult> GetPaginated(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? searchTerm = null,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool sortDescending = false,
        CancellationToken cancellationToken = default)
    {
        var query = new GetProductCategoryPaginatedQuery(pageNumber, pageSize, searchTerm, sortBy, sortDescending);
        var result = await messageBus.InvokeAsync<PaginatedResult<ProductCategory>>(query, cancellationToken);
        
        return Results.Ok(PaginatedResult<ProductCategory>.MapFrom(result, ProductCategoryDto.FromDomainModel));
    }

    [HttpGet("api/product-categories/all")]
    public async Task<IResult> GetAll(CancellationToken cancellationToken)
    {
        // Цей ендпойнт потрібен для випадаючих списків (дропдаунів) на фронтенді
        var query = new GetAllProductCategoriesQuery();
        var result = await messageBus.InvokeAsync<IReadOnlyList<ProductCategory>>(query, cancellationToken);
        
        return Results.Ok(result.Select(ProductCategoryDto.FromDomainModel));
    }

    [HttpGet("api/product-categories/{id:guid}")]
    public async Task<IResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetProductCategoryByIdQuery(id);
        var result = await messageBus.InvokeAsync<Either<ProductCategoryException, ProductCategory>>(query, cancellationToken);
        
        return result.Match<IResult>(
            category => Results.Ok(ProductCategoryDto.FromDomainModel(category)),
            ex => ex.ToIResult());
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("api/product-categories")]
    public async Task<IResult> Create([FromBody] ProductCategoryCreateDto request, CancellationToken cancellationToken)
    {
        var cmd = new CreateProductCategoryCommand(request.NameUk, request.NameEn);
        var result = await messageBus.InvokeAsync<Either<ProductCategoryException, ProductCategory>>(cmd, cancellationToken);
        
        return result.Match<IResult>(
            category => Results.Created($"/api/product-categories/{category.Id.Value}", ProductCategoryDto.FromDomainModel(category)),
            ex => ex.ToIResult());
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("api/product-categories/{id:guid}")]
    public async Task<IResult> Update(Guid id, [FromBody] ProductCategoryUpdateDto request, CancellationToken cancellationToken)
    {
        var cmd = new UpdateProductCategoryCommand(id, request.NameUk, request.NameEn);
        var result = await messageBus.InvokeAsync<Either<ProductCategoryException, ProductCategory>>(cmd, cancellationToken);
        
        return result.Match<IResult>(
            category => Results.Ok(ProductCategoryDto.FromDomainModel(category)),
            ex => ex.ToIResult());
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("api/product-categories/{id:guid}")]
    public async Task<IResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var cmd = new DeleteProductCategoryCommand(id);
        var result = await messageBus.InvokeAsync<Either<ProductCategoryException, ProductCategory>>(cmd, cancellationToken);
        
        return result.Match<IResult>(
            category => Results.Ok(ProductCategoryDto.FromDomainModel(category)),
            ex => ex.ToIResult());
    }
}