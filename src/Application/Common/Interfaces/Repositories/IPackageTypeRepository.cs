using Domain.PackageTypes;

namespace Application.Common.Interfaces.Repositories;

public interface IPackageTypeRepository
{
    Task<PackageType> Add(PackageType packageType, CancellationToken cancellationToken);
    Task<PackageType> Update(PackageType packageType, CancellationToken cancellationToken);
    Task<PackageType> Delete(PackageType packageType, CancellationToken cancellationToken);
}
