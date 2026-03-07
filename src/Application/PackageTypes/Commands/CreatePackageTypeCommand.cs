using Application.Common.Interfaces.Repositories;
using Application.Common.Interfaces.Services;
using Application.PackageTypes.Exceptions;
using Domain.PackageTypes;
using LanguageExt;
using Microsoft.AspNetCore.Http;

namespace Application.PackageTypes.Commands;

public record CreatePackageTypeCommand(
    string TitleUk,
    string TitleEn,
    IFormFile ImageIcon);

public static class CreatePackageTypeCommandHandler
{
    public static async Task<Either<PackageTypeException, PackageType>> Handle(
        CreatePackageTypeCommand command,
        IPackageTypeRepository packageTypeRepository,
        IFileService fileService,
        CancellationToken cancellationToken)
    {
        try
        {
            const string requestPath = "/uploads";

            var iconFileName = await fileService.SaveFileAsync(command.ImageIcon, cancellationToken);
            var iconUrl = $"{requestPath}/{iconFileName}";

            var title = new Domain.LocalizedString(command.TitleUk, command.TitleEn);
            var packageType = PackageType.New(title, iconUrl);
            return await packageTypeRepository.Add(packageType, cancellationToken);
        }
        catch (Exception ex)
        {
            return new PackageTypeUnknownException(Guid.Empty, ex);
        }
    }
}
