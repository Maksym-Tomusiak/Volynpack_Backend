using Application.Common.Interfaces.Queries;
using Domain.PackageMaterials;

namespace Application.PackageMaterials.Queries;

public record GetAllPackageMaterialsQuery;

public static class GetAllPackageMaterialsQueryHandler
{
    public static async Task<IReadOnlyList<PackageMaterial>> Handle(
        GetAllPackageMaterialsQuery query,
        IPackageMaterialQueries packageMaterialQueries,
        CancellationToken cancellationToken)
    {
        return await packageMaterialQueries.GetAll(cancellationToken);
    }
}
