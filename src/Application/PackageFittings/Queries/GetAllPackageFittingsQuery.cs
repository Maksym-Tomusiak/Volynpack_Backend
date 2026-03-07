using Application.Common.Interfaces.Queries;
using Domain.PackageFittings;

namespace Application.PackageFittings.Queries;

public record GetAllPackageFittingsQuery;

public static class GetAllPackageFittingsQueryHandler
{
    public static async Task<IReadOnlyList<PackageFitting>> Handle(
        GetAllPackageFittingsQuery query,
        IPackageFittingQueries packageFittingQueries,
        CancellationToken cancellationToken)
    {
        return await packageFittingQueries.GetAll(cancellationToken);
    }
}
