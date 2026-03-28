using Application.Common.Interfaces.Queries;
using Application.PackageFittings.Exceptions;
using Domain.PackageFittings;
using Domain.PackageMaterials;
using Domain.PackageTypes;
using LanguageExt;

namespace Application.PackageFittings.Queries;

public record class GetPackageFittingByTypeAndMaterialQuery(Guid TypeId, Guid MaterialId);

public static class GetPackageFittingByTypeAndMaterialQueryHandler
{
    public static async Task<Either<PackageFittingException, PackageFitting>> Handle(
        GetPackageFittingByTypeAndMaterialQuery query,
        IPackageFittingQueries packageFittingQueries,
        CancellationToken cancellationToken)
    {
        var typeId = new PackageTypeId(query.TypeId);
        var materialId = new PackageMaterialId(query.MaterialId);
        var result = await packageFittingQueries.GetByTypeAndMaterial(typeId, materialId, cancellationToken);
        return result.Match<Either<PackageFittingException, PackageFitting>>(
            fitting => fitting,
            () => new PackageFittingNotFoundException(Guid.Empty));
    }
}