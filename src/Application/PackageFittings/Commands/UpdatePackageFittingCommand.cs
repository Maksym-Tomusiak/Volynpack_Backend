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

public record UpdatePackageFittingCommand(
    Guid Id,
    Guid TypeId,
    Guid MaterialId,
    IFormFile? FittingImage);

public static class UpdatePackageFittingCommandHandler
{
    public static async Task<Either<PackageFittingException, PackageFitting>> Handle(
        UpdatePackageFittingCommand command,
        IPackageFittingRepository packageFittingRepository,
        IPackageFittingQueries packageFittingQueries,
        IPackageTypeQueries packageTypeQueries,
        IPackageMaterialQueries packageMaterialQueries,
        IFileService fileService,
        CancellationToken cancellationToken)
    {
        var fittingId = new PackageFittingId(command.Id);
        var existing = await packageFittingQueries.GetById(fittingId, cancellationToken);
        if (existing.IsNone)
            return new PackageFittingNotFoundException(command.Id);

        var typeId = new PackageTypeId(command.TypeId);
        var type = await packageTypeQueries.GetById(typeId, cancellationToken);
        if (type.IsNone)
            return new PackageFittingUnknownException(command.Id, new Exception($"PackageType under id: {command.TypeId} not found!"));

        var materialId = new PackageMaterialId(command.MaterialId);
        var material = await packageMaterialQueries.GetById(materialId, cancellationToken);
        if (material.IsNone)
            return new PackageFittingUnknownException(command.Id, new Exception($"PackageMaterial under id: {command.MaterialId} not found!"));

        try
        {
            var fitting = existing.IfNoneUnsafe((PackageFitting)null!)!;
            const string requestPath = "/uploads/fittings";

            string imageUrl;
            if (command.FittingImage is not null)
            {
                if (!string.IsNullOrEmpty(fitting.FittingImageUrl))
                    await fileService.DeleteFileAsync(fitting.FittingImageUrl, "fittings", cancellationToken);

                var fileName = await fileService.SaveFileAsync(command.FittingImage, "fittings", cancellationToken);
                imageUrl = $"{requestPath}/{fileName}";
            }
            else
            {
                imageUrl = fitting.FittingImageUrl;
            }

            fitting.Update(typeId, materialId, imageUrl);
            return await packageFittingRepository.Update(fitting, cancellationToken);
        }
        catch (Exception ex)
        {
            return new PackageFittingUnknownException(command.Id, ex);
        }
    }
}
