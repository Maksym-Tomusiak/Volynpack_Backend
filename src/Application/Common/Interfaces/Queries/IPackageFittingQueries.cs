using Application.Common.Models;
using Domain.PackageFittings;
using Domain.PackageMaterials;
using Domain.PackageTypes;
using LanguageExt;

namespace Application.Common.Interfaces.Queries;

public interface IPackageFittingQueries
{
    Task<IReadOnlyList<PackageFitting>> GetAll(CancellationToken cancellationToken);
    Task<Option<PackageFitting>> GetById(PackageFittingId id, CancellationToken cancellationToken);
    Task<Option<PackageFitting>> GetByTypeAndMaterial(PackageTypeId typeId, PackageMaterialId materialId, CancellationToken cancellationToken);
    Task<PaginatedResult<PackageFitting>> GetPaginated(PaginationParameters parameters, CancellationToken cancellationToken);
}
