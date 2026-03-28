using Application.Common.Interfaces.Queries;
using Application.PackageMaterials.Exceptions;
using Domain.PackageMaterials;
using LanguageExt;

namespace Application.PackageMaterials.Queries;

public record GetPackageMaterialByIdQuery(Guid Id);

public static class GetPackageMaterialByIdQueryHandler
{
    public static async Task<Either<PackageMaterialException, PackageMaterial>> Handle(
        GetPackageMaterialByIdQuery query,
        IPackageMaterialQueries packageMaterialQueries,
        CancellationToken cancellationToken)
    {
        var materialId = new PackageMaterialId(query.Id);
        var result = await packageMaterialQueries.GetById(materialId, cancellationToken);
        return result.Match<Either<PackageMaterialException, PackageMaterial>>(
            material => material,
            () => new PackageMaterialNotFoundException(query.Id));
    }
}
