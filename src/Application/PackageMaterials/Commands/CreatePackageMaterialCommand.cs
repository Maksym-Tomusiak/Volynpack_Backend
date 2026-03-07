using Application.Common.Interfaces.Repositories;
using Application.PackageMaterials.Exceptions;
using Domain.PackageMaterials;
using LanguageExt;

namespace Application.PackageMaterials.Commands;

public record CreatePackageMaterialCommand(string TitleUk, string TitleEn);

public static class CreatePackageMaterialCommandHandler
{
    public static async Task<Either<PackageMaterialException, PackageMaterial>> Handle(
        CreatePackageMaterialCommand command,
        IPackageMaterialRepository packageMaterialRepository,
        CancellationToken cancellationToken)
    {
        try
        {
            var title = new Domain.LocalizedString(command.TitleUk, command.TitleEn);
            var material = PackageMaterial.New(title);
            return await packageMaterialRepository.Add(material, cancellationToken);
        }
        catch (Exception ex)
        {
            return new PackageMaterialUnknownException(Guid.Empty, ex);
        }
    }
}
