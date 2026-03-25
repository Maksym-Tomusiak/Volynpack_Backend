using Api.Dtos;
using Api.Hubs;
using Api.Modules.Errors;
using Application.Common.Models;
using Application.Orders.Commands;
using Application.Orders.Exceptions;
using Application.Orders.Queries;
using Domain.Orders;
using Domain.OrderStatuses;
using Domain.PrintingOptions;
using Domain.DelivaryMethods;
using LanguageExt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.SignalR;
using Wolverine;

namespace Api.Controllers;

[EnableRateLimiting("CatalogPolicy")]
[ApiController]
public class OrdersController(IMessageBus messageBus, IHubContext<NotificationHub> hubContext) : ControllerBase
{
    // Отримання списку всіх статусів (для адмінки)
    [Authorize(Roles = "Admin")]
    [HttpGet("api/orders/statuses")]
    public async Task<IResult> GetOrderStatuses(CancellationToken cancellationToken)
    {
        var query = new GetOrderStatusesQuery(); // Переконайся, що в тебе є такий Query
        var result = await messageBus.InvokeAsync<IReadOnlyList<OrderStatus>>(query, cancellationToken);
        return Results.Ok(result.Select(OrderStatusDto.FromDomainModel));
    }

    // Отримання списку опцій друку (для клієнта)
    [HttpGet("api/orders/printing-options")]
    public async Task<IResult> GetPrintingOptions(CancellationToken cancellationToken)
    {
        var query = new GetPrintingOptionsQuery(); // Переконайся, що в тебе є такий Query
        var result = await messageBus.InvokeAsync<IReadOnlyList<PrintingOption>>(query, cancellationToken);
        return Results.Ok(result.Select(PrintingOptionDto.FromDomainModel));
    }

    // Отримання списку методів доставки (для клієнта)
    [HttpGet("api/orders/delivery-methods")]
    public async Task<IResult> GetDeliveryMethods(CancellationToken cancellationToken)
    {
        var query = new GetDeliveryMethodsQuery();
        var result = await messageBus.InvokeAsync<IReadOnlyList<DeliveryMethod>>(query, cancellationToken);
        return Results.Ok(result.Select(DeliveryMethodDto.FromDomainModel));
    }

    // Пагінація замовлень (для адмінки)
    [Authorize(Roles = "Admin")]
    [HttpGet("api/orders")]
    public async Task<IResult> GetOrdersPaginated(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? searchTerm = null,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool sortDescending = false,
        CancellationToken cancellationToken = default)
    {
        var query = new GetOrdersPaginatedQuery(pageNumber, pageSize, searchTerm, sortBy, sortDescending);
        var result = await messageBus.InvokeAsync<PaginatedResult<Order>>(query, cancellationToken);
        return Results.Ok(PaginatedResult<Order>.MapFrom(result, OrderDto.FromDomainModel));
    }

    // Деталі конкретного замовлення
    [Authorize(Roles = "Admin")]
    [HttpGet("api/orders/{id:guid}")]
    public async Task<IResult> GetOrderById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetOrderByIdQuery(id);
        var result = await messageBus.InvokeAsync<Either<OrderException, Order>>(query, cancellationToken);
        return result.Match<IResult>(
            order => Results.Ok(OrderDto.FromDomainModel(order)),
            ex => ex.ToIResult());
    }

    [HttpPost("api/orders")]
    public async Task<IResult> CreateOrder([FromBody] CreateOrderDto request, CancellationToken cancellationToken)
    {
        var cmd = new CreateOrderCommand(
            request.Items.Select(i => new CreateOrderItemCommand(i.ProductVariantId, i.Quantity, i.PrintingOptionId)), 
            request.DeliveryMethodId,
            request.FullName,
            request.PhoneNumber,
            request.Town,
            request.Branch);

        var result = await messageBus.InvokeAsync<Either<OrderException, Order>>(cmd, cancellationToken);
        return await result.MatchAsync<IResult>(
            async order =>
            {
                await hubContext.Clients.All.SendAsync("OrderCreated", OrderDto.FromDomainModel(order), cancellationToken);
                return Results.Created($"/api/orders/{order.Id.Value}", OrderDto.FromDomainModel(order));
            },
            ex => Task.FromResult(ex.ToIResult()));
    }

    // Оновлення замовлення (для адмінки)
    [Authorize(Roles = "Admin")]
    [HttpPut("api/orders/{id:guid}")]
    public async Task<IResult> UpdateOrder(Guid id, [FromBody] UpdateOrderDto request, CancellationToken cancellationToken)
    {
        var cmd = new UpdateOrderCommand(
            id, 
            request.OrderStatusId,
            request.DeliveryMethodId,
            request.FullName,
            request.PhoneNumber,
            request.Town,
            request.Branch,
            request.Items.Select(i => (i.ProductVariantId, i.Quantity, i.PrintingOptionId)));
        var result = await messageBus.InvokeAsync<Either<OrderException, Order>>(cmd, cancellationToken);
        return await result.MatchAsync<IResult>(
            async order =>
            {
                await hubContext.Clients.All.SendAsync("OrderUpdated", OrderDto.FromDomainModel(order), cancellationToken);
                return Results.Ok(OrderDto.FromDomainModel(order));
            },
            ex => Task.FromResult(ex.ToIResult()));
    }

    // Видалення замовлення
    [Authorize(Roles = "Admin")]
    [HttpDelete("api/orders/{id:guid}")]
    public async Task<IResult> DeleteOrder(Guid id, CancellationToken cancellationToken)
    {
        var cmd = new DeleteOrderCommand(id);
        var result = await messageBus.InvokeAsync<Either<OrderException, Order>>(cmd, cancellationToken);
        return result.Match<IResult>(
            order => Results.Ok(OrderDto.FromDomainModel(order)),
            ex => ex.ToIResult());
    }
}
