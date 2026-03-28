using Application.Common.Models;
using Domain.PackageMaterials;
using LanguageExt;

namespace Application.Common.Interfaces.Queries;

public interface IPackageMaterialQueries
{
    Task<IReadOnlyList<PackageMaterial>> GetAll(CancellationToken cancellationToken);
    Task<Option<PackageMaterial>> GetById(PackageMaterialId id, CancellationToken cancellationToken);
    Task<PaginatedResult<PackageMaterial>> GetPaginated(PaginationParameters parameters, CancellationToken cancellationToken);
}
