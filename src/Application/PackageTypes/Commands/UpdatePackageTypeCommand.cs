using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Common.Interfaces.Services;
using Application.PackageTypes.Exceptions;
using Domain.PackageTypes;
using LanguageExt;
using Microsoft.AspNetCore.Http;

namespace Application.PackageTypes.Commands;

public record UpdatePackageTypeCommand(
    Guid Id,
    string TitleUk,
    string TitleEn,
    IFormFile? ImageIcon);

public static class UpdatePackageTypeCommandHandler
{
    public static async Task<Either<PackageTypeException, PackageType>> Handle(
        UpdatePackageTypeCommand command,
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
            const string requestPath = "/uploads";

            string iconUrl;
            if (command.ImageIcon is not null)
            {
                if (!string.IsNullOrEmpty(packageType.ImageIconUrl))
                    await fileService.DeleteFileAsync(packageType.ImageIconUrl, cancellationToken);

                var iconFileName = await fileService.SaveFileAsync(command.ImageIcon, cancellationToken);
                iconUrl = $"{requestPath}/{iconFileName}";
            }
            else
            {
                iconUrl = packageType.ImageIconUrl;
            }

            var title = new Domain.LocalizedString(command.TitleUk, command.TitleEn);
            packageType.Update(title, iconUrl);
            return await packageTypeRepository.Update(packageType, cancellationToken);
        }
        catch (Exception ex)
        {
            return new PackageTypeUnknownException(command.Id, ex);
        }
    }
}
