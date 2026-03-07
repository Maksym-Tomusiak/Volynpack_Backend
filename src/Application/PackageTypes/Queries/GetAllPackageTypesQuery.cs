using Application.Common.Interfaces.Queries;
using Domain.PackageTypes;

namespace Application.PackageTypes.Queries;

public record GetAllPackageTypesQuery;

public static class GetAllPackageTypesQueryHandler
{
    public static async Task<IReadOnlyList<PackageType>> Handle(
        GetAllPackageTypesQuery query,
        IPackageTypeQueries packageTypeQueries,
        CancellationToken cancellationToken)
    {
        return await packageTypeQueries.GetAll(cancellationToken);
    }
}
