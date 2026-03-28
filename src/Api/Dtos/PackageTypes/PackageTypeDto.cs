using Domain.PackageTypes;
using Microsoft.AspNetCore.Http;

namespace Api.Dtos.PackageTypes;

public record PackageTypeDto(Guid Id, LocalizedStringDto Title, string ImageIconUrl)
{
    public static PackageTypeDto FromDomainModel(PackageType packageType) =>
        new(packageType.Id.Value,
            new LocalizedStringDto(packageType.Title.Uk, packageType.Title.En),
            packageType.ImageIconUrl);
}

public record PackageTypeCreateDto(string TitleUk, string TitleEn, IFormFile ImageIcon);
public record PackageTypeUpdateDto(string TitleUk, string TitleEn, IFormFile? ImageIcon);
