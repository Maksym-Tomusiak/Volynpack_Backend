using Domain.PackageMaterials;
using Microsoft.Extensions.Localization;

namespace Api.Dtos.PackageMaterials;

public record PackageMaterialDto(Guid Id, LocalizedStringDto Title, LocalizedStringDto Description)
{
    public static PackageMaterialDto FromDomainModel(PackageMaterial material) =>
        new(material.Id.Value,
            new LocalizedStringDto(material.Title.Uk, material.Title.En),
            new LocalizedStringDto(material.Description.Uk, material.Description.En));
}

public record PackageMaterialCreateDto(string TitleUk, string TitleEn, string DescriptionUk, string DescriptionEn);
public record PackageMaterialUpdateDto(string TitleUk, string TitleEn, string DescriptionUk, string DescriptionEn);
