using Domain.OrderStatuses;
using LanguageExt;

namespace Application.Common.Interfaces.Queries;

public interface IOrderStatusQueries
{
    Task<IReadOnlyList<OrderStatus>> GetAll(CancellationToken cancellationToken);
    Task<Option<OrderStatus>> GetByName(string name, CancellationToken cancellationToken);
}