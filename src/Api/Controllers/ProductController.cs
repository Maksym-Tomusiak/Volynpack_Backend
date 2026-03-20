using Api.Dtos.Products;
using Api.Modules.Errors;
using Application.Common.Models;
using Application.Products.Commands;
using Application.Products.Exceptions;
using Application.Products.Queries;
using Domain.Products;
using LanguageExt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wolverine;

namespace Api.Controllers;

[ApiController]
public class ProductController(IMessageBus messageBus) : ControllerBase
{
    [HttpGet("api/products")]
    public async Task<IResult> GetPaginated(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? searchTerm = null,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool sortDescending = false,
        CancellationToken cancellationToken = default)
    {
        var query = new GetProductPaginatedQuery(pageNumber, pageSize, searchTerm, sortBy, sortDescending);
        var result = await messageBus.InvokeAsync<PaginatedResult<Product>>(query, cancellationToken);
        
        return Results.Ok(PaginatedResult<Product>.MapFrom(result, ProductDto.FromDomainModel));
    }

    [HttpGet("api/products/{id:guid}")]
    public async Task<IResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetProductByIdQuery(id);
        var result = await messageBus.InvokeAsync<Either<ProductException, Product>>(query, cancellationToken);
        
        return result.Match<IResult>(
            product => Results.Ok(ProductDto.FromDomainModel(product)),
            ex => ex.ToIResult());
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("api/products")]
    public async Task<IResult> Create([FromBody] ProductCreateDto request, CancellationToken cancellationToken)
    {
        var cmd = new CreateProductCommand(
            request.TypeId,
            request.TitleUk,
            request.TitleEn,
            request.DescriptionUk,
            request.DescriptionEn,
            request.CategoryIds,
            request.SuitableFor.Select(f => new TextFeatureDto(f.TitleUk, f.TitleEn, f.DescriptionUk, f.DescriptionEn)).ToList(),
            request.GeneralCharacteristics.Select(f => new TextFeatureDto(f.TitleUk, f.TitleEn, f.DescriptionUk, f.DescriptionEn)).ToList()
        );

        var result = await messageBus.InvokeAsync<Either<ProductException, Product>>(cmd, cancellationToken);
        
        return result.Match<IResult>(
            product => Results.Created($"/api/products/{product.Id.Value}", ProductDto.FromDomainModel(product)),
            ex => ex.ToIResult());
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("api/products/{id:guid}")]
    public async Task<IResult> Update(Guid id, [FromBody] ProductUpdateDto request, CancellationToken cancellationToken)
    {
        var cmd = new UpdateProductCommand(
            id,
            request.TypeId,
            request.TitleUk,
            request.TitleEn,
            request.DescriptionUk,
            request.DescriptionEn,
            request.CategoryIds,
            request.SuitableFor.Select(f => new TextFeatureDto(f.TitleUk, f.TitleEn, f.DescriptionUk, f.DescriptionEn)).ToList(),
            request.GeneralCharacteristics.Select(f => new TextFeatureDto(f.TitleUk, f.TitleEn, f.DescriptionUk, f.DescriptionEn)).ToList()
        );

        var result = await messageBus.InvokeAsync<Either<ProductException, Product>>(cmd, cancellationToken);
        
        return result.Match<IResult>(
            product => Results.Ok(ProductDto.FromDomainModel(product)),
            ex => ex.ToIResult());
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("api/products/{id:guid}")]
    public async Task<IResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var cmd = new DeleteProductCommand(id);
        var result = await messageBus.InvokeAsync<Either<ProductException, Product>>(cmd, cancellationToken);
        
        return result.Match<IResult>(
            product => Results.Ok(ProductDto.FromDomainModel(product)),
            ex => ex.ToIResult());
    }
}