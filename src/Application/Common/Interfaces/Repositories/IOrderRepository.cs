using Domain.Orders;

namespace Application.Common.Interfaces.Repositories;

public interface IOrderRepository
{
    Task<Order> Add(Order order, CancellationToken cancellationToken);
    Task<Order> Update(Order order, CancellationToken cancellationToken);
    Task<Order> Delete(Order order, CancellationToken cancellationToken);
}