using Domain.PackageFittings;

namespace Application.Common.Interfaces.Repositories;

public interface IPackageFittingRepository
{
    Task<PackageFitting> Add(PackageFitting fitting, CancellationToken cancellationToken);
    Task<PackageFitting> Update(PackageFitting fitting, CancellationToken cancellationToken);
    Task<PackageFitting> Delete(PackageFitting fitting, CancellationToken cancellationToken);
}
