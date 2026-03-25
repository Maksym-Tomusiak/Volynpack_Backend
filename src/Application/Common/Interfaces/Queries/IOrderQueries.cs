using Application.Common.Models;
using Domain.Orders;
using LanguageExt;

namespace Application.Common.Interfaces.Queries;

public interface IOrderQueries
{
    Task<Option<Order>> GetById(OrderId id, CancellationToken cancellationToken);
    Task<PaginatedResult<Order>> GetPaginated(PaginationParameters parameters, CancellationToken cancellationToken);
}