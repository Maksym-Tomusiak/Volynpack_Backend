using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Common.Models;
using Domain.Orders;
using LanguageExt;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class OrderRepository(ApplicationDbContext context) : IOrderRepository, IOrderQueries
{
    public async Task<Order> Add(Order order, CancellationToken cancellationToken)
    {
        await context.Orders.AddAsync(order, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return order;
    }

    public async Task<Order> Update(Order order, CancellationToken cancellationToken)
    {
        var tracked = await context.Orders
            .Include(x => x.Items)
            .FirstAsync(x => x.Id == order.Id, cancellationToken);

        context.Entry(tracked).CurrentValues.SetValues(order);
        
        // Synchronize Items precisely
        var existingItems = tracked.Items.ToList();
        var newItems = order.Items.ToList();

        // 1. Remove items that are not in the new list
        foreach (var existing in existingItems)
        {
            if (!newItems.Any(n => n.ProductVariantId == existing.ProductVariantId && n.PrintingOptionId == existing.PrintingOptionId))
            {
                context.Remove(existing);
            }
        }

        // 2. Add or Update
        foreach (var newItem in newItems)
        {
            var existing = existingItems.FirstOrDefault(e => e.ProductVariantId == newItem.ProductVariantId && e.PrintingOptionId == newItem.PrintingOptionId);
            
            if (existing != null)
            {
                // Update existing item's quantity
                existing.ChangeQuantity(newItem.Quantity);
            }
            else
            {
                // Add new item (linked to tracked order)
                tracked.Items.Add(newItem);
            }
        }
        
        await context.SaveChangesAsync(cancellationToken);
        
        return tracked;
    }

    public async Task<Order> Delete(Order order, CancellationToken cancellationToken)
    {
        context.Orders.Remove(order);
        await context.SaveChangesAsync(cancellationToken);
        return order;
    }

    public async Task<Option<Order>> GetById(OrderId id, CancellationToken cancellationToken)
    {
        var entity = await context.Orders
            .AsNoTracking()
            .Include(x => x.OrderStatus)
            .Include(x => x.DeliveryMethod)
            .Include(x => x.Items)
                .ThenInclude(i => i.ProductVariant)
                    .ThenInclude(pv => pv!.Product)
            .Include(x => x.Items)
                .ThenInclude(i => i.ProductVariant)
                    .ThenInclude(pv => pv!.Material)
            .Include(x => x.Items)
                .ThenInclude(i => i.PrintingOption)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return entity is null ? Option<Order>.None : Option<Order>.Some(entity);
    }

    public async Task<PaginatedResult<Order>> GetPaginated(PaginationParameters parameters, CancellationToken cancellationToken)
    {
        var query = context.Orders
            .AsNoTracking()
            .Include(x => x.OrderStatus)
            .Include(x => x.DeliveryMethod)
            .Include(x => x.Items)
                .ThenInclude(i => i.ProductVariant)
                    .ThenInclude(pv => pv!.Product)
            .Include(x => x.Items)
                .ThenInclude(i => i.ProductVariant)
                    .ThenInclude(pv => pv!.Material)
            .Include(x => x.Items)
                .ThenInclude(i => i.PrintingOption)
            .AsQueryable();

        // Пошук
        if (!string.IsNullOrWhiteSpace(parameters.SearchTerm))
        {
            var term = parameters.SearchTerm.ToLower();
            
            // Наприклад, пошук по ID замовлення або імені статусу
            query = query.Where(x => 
                x.Id.Value.ToString().Contains(term) || 
                EF.Functions.ILike(x.OrderStatus!.Name, $"%{term}%") ||
                EF.Functions.ILike(x.FullName, $"%{term}%"));
        }

        // Сортування
        if (!string.IsNullOrWhiteSpace(parameters.SortBy))
        {
            query = parameters.SortBy.ToLower() switch
            {
                "createdat" => parameters.SortDescending 
                    ? query.OrderByDescending(x => x.CreatedAt) 
                    : query.OrderBy(x => x.CreatedAt),
                "status" => parameters.SortDescending 
                    ? query.OrderByDescending(x => x.OrderStatus!.Name) 
                    : query.OrderBy(x => x.OrderStatus!.Name),
                "quantity" => parameters.SortDescending 
                    ? query.OrderByDescending(x => x.Items.Sum(i => i.Quantity)) 
                    : query.OrderBy(x => x.Items.Sum(i => i.Quantity)),
                _ => query.OrderByDescending(x => x.CreatedAt)
            };
        }
        else
        {
            query = query.OrderByDescending(x => x.CreatedAt);
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var totalPages = (int)Math.Ceiling(totalCount / (double)parameters.PageSize);

        var items = await query
            .Skip((parameters.PageNumber - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedResult<Order>(items, totalCount, parameters.PageNumber, parameters.PageSize, totalPages);
    }
}