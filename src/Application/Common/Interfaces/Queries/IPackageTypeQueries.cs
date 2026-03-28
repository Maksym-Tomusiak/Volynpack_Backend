using Application.Common.Models;
using Domain.PackageTypes;
using LanguageExt;

namespace Application.Common.Interfaces.Queries;

public interface IPackageTypeQueries
{
    Task<IReadOnlyList<PackageType>> GetAll(CancellationToken cancellationToken);
    Task<Option<PackageType>> GetById(PackageTypeId id, CancellationToken cancellationToken);
    Task<PaginatedResult<PackageType>> GetPaginated(PaginationParameters parameters, CancellationToken cancellationToken);
}
