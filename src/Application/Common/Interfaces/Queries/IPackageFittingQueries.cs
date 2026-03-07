using Application.Common.Models;
using Domain.PackageFittings;
using LanguageExt;

namespace Application.Common.Interfaces.Queries;

public interface IPackageFittingQueries
{
    Task<IReadOnlyList<PackageFitting>> GetAll(CancellationToken cancellationToken);
    Task<Option<PackageFitting>> GetById(PackageFittingId id, CancellationToken cancellationToken);
    Task<PaginatedResult<PackageFitting>> GetPaginated(PaginationParameters parameters, CancellationToken cancellationToken);
}
