using Domain.DelivaryMethods;

namespace Application.Common.Interfaces.Queries;

public interface IDeliveryMethodQueries
{
    Task<IReadOnlyList<DeliveryMethod>> GetAll(CancellationToken cancellationToken);
}
