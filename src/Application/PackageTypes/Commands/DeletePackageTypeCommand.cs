using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Common.Interfaces.Services;
using Application.PackageTypes.Exceptions;
using Domain.PackageTypes;
using LanguageExt;

namespace Application.PackageTypes.Commands;

public record DeletePackageTypeCommand(Guid Id);

public static class DeletePackageTypeCommandHandler
{
    public static async Task<Either<PackageTypeException, PackageType>> Handle(
        DeletePackageTypeCommand command,
        IPackageTypeRepository packageTypeRepository,
        IPackageTypeQueries packageTypeQueries,
        IFileService fileService,
        CancellationToken cancellationToken)
    {
        var packageTypeId = new PackageTypeId(command.Id);
        var existing = await packageTypeQueries.GetById(packageTypeId, cancellationToken);
        if (existing.IsNone)
            return new PackageTypeNotFoundException(command.Id);

        try
        {
            var packageType = existing.IfNoneUnsafe((PackageType)null!)!;

            // Delete associated image files
            if (!string.IsNullOrEmpty(packageType.ImageIconUrl))
                await fileService.DeleteFileAsync(packageType.ImageIconUrl, cancellationToken);

            if (!string.IsNullOrEmpty(packageType.ImageOverlayUrl))
                await fileService.DeleteFileAsync(packageType.ImageOverlayUrl, cancellationToken);

            return await packageTypeRepository.Delete(packageType, cancellationToken);
        }
        catch (Exception ex)
        {
            return new PackageTypeUnknownException(command.Id, ex);
        }
    }
}
