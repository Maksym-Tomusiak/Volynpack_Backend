using Domain.PackageMaterials;

namespace Application.Common.Interfaces.Repositories;

public interface IPackageMaterialRepository
{
    Task<PackageMaterial> Add(PackageMaterial material, CancellationToken cancellationToken);
    Task<PackageMaterial> Update(PackageMaterial material, CancellationToken cancellationToken);
    Task<PackageMaterial> Delete(PackageMaterial material, CancellationToken cancellationToken);
}
