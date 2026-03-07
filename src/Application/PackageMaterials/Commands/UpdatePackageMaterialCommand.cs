using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.PackageMaterials.Exceptions;
using Domain.PackageMaterials;
using LanguageExt;

namespace Application.PackageMaterials.Commands;

public record UpdatePackageMaterialCommand(Guid Id, string TitleUk, string TitleEn);

public static class UpdatePackageMaterialCommandHandler
{
    public static async Task<Either<PackageMaterialException, PackageMaterial>> Handle(
        UpdatePackageMaterialCommand command,
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
            var title = new Domain.LocalizedString(command.TitleUk, command.TitleEn);
            material.Update(title);
            return await packageMaterialRepository.Update(material, cancellationToken);
        }
        catch (Exception ex)
        {
            return new PackageMaterialUnknownException(command.Id, ex);
        }
    }
}
