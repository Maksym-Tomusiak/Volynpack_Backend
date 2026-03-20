using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Common.Interfaces.Services;
using Application.PackageFittings.Exceptions;
using Domain.PackageFittings;
using Domain.PackageMaterials;
using Domain.PackageTypes;
using LanguageExt;
using Microsoft.AspNetCore.Http;

namespace Application.PackageFittings.Commands;

public record CreatePackageFittingCommand(
    Guid TypeId,
    Guid MaterialId,
    IFormFile FittingImage);

public static class CreatePackageFittingCommandHandler
{
    public static async Task<Either<PackageFittingException, PackageFitting>> Handle(
        CreatePackageFittingCommand command,
        IPackageFittingRepository packageFittingRepository,
        IPackageTypeQueries packageTypeQueries,
        IPackageMaterialQueries packageMaterialQueries,
        IFileService fileService,
        CancellationToken cancellationToken)
    {
        var typeId = new PackageTypeId(command.TypeId);
        var type = await packageTypeQueries.GetById(typeId, cancellationToken);
        if (type.IsNone)
            return new PackageFittingUnknownException(Guid.Empty, new Exception($"PackageType under id: {command.TypeId} not found!"));

        var materialId = new PackageMaterialId(command.MaterialId);
        var material = await packageMaterialQueries.GetById(materialId, cancellationToken);
        if (material.IsNone)
            return new PackageFittingUnknownException(Guid.Empty, new Exception($"PackageMaterial under id: {command.MaterialId} not found!"));

        try
        {
            const string requestPath = "/uploads/fittings";
            var fileName = await fileService.SaveFileAsync(command.FittingImage, "fittings", cancellationToken);
            var imageUrl = $"{requestPath}/{fileName}";

            var fitting = PackageFitting.New(typeId, materialId, imageUrl);
            return await packageFittingRepository.Add(fitting, cancellationToken);
        }
        catch (Exception ex)
        {
            return new PackageFittingUnknownException(Guid.Empty, ex);
        }
    }
}
