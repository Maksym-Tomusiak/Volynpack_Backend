using Domain.PackageMaterials;

namespace Api.Dtos.PackageMaterials;

public record PackageMaterialDto(Guid Id, LocalizedStringDto Title)
{
    public static PackageMaterialDto FromDomainModel(PackageMaterial material) =>
        new(material.Id.Value,
            new LocalizedStringDto(material.Title.Uk, material.Title.En));
}

public record PackageMaterialCreateDto(string TitleUk, string TitleEn);
public record PackageMaterialUpdateDto(string TitleUk, string TitleEn);
