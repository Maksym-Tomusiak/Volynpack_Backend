using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.PackageMaterials.Exceptions;
using Domain.PackageMaterials;
using LanguageExt;

namespace Application.PackageMaterials.Commands;

public record DeletePackageMaterialCommand(Guid Id);

public static class DeletePackageMaterialCommandHandler
{
    public static async Task<Either<PackageMaterialException, PackageMaterial>> Handle(
        DeletePackageMaterialCommand command,
        IPackageMaterialRepository packageMaterialRepository,
        IPackageMaterialQueries packageMaterialQueries,
        CancellationToken cancellationToken)
    {
        var materialId = new PackageMaterialId(command.Id);
        var existing = await packageMaterialQueries.GetById(materialId, cancellationToken);
        if (existing.IsNone)
            return new PackageMaterialNotFoundException(command.Id);

        try
        {
            var material = existing.IfNoneUnsafe((PackageMaterial)null!)!;
            return await packageMaterialRepository.Delete(material, cancellationToken);
        }
        catch (Exception ex)
        {
            return new PackageMaterialUnknownException(command.Id, ex);
        }
    }
}
