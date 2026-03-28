using Api.Dtos;
using Api.Dtos.PackageMaterials;
using Api.Dtos.PackageTypes;
using Domain.PackageFittings;
using Microsoft.AspNetCore.Http;

namespace Api.Dtos.PackageFittings;

public record PackageFittingDto(
    Guid Id,
    PackageTypeDto? Type,
    PackageMaterialDto? Material,
    string FittingImageUrl)
{
    public static PackageFittingDto FromDomainModel(PackageFitting fitting) =>
        new(fitting.Id.Value,
            fitting.Type is null ? null : PackageTypeDto.FromDomainModel(fitting.Type),
            fitting.Material is null ? null : PackageMaterialDto.FromDomainModel(fitting.Material),
            fitting.FittingImageUrl);
}

public record PackageFittingCreateDto(Guid TypeId, Guid MaterialId, IFormFile FittingImage);
public record PackageFittingUpdateDto(Guid TypeId, Guid MaterialId, IFormFile? FittingImage);
