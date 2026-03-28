using Domain.PackageMaterials;
using Domain.PackageTypes;

namespace Domain.PackageFittings;

public class PackageFitting
{
    public PackageFittingId Id { get; private set; }
    public PackageTypeId TypeId { get; set; }
    public PackageType? Type { get; set; }
    public PackageMaterialId MaterialId { get; set; }
    public PackageMaterial? Material { get; set; }
    public string FittingImageUrl { get; set; }

    private PackageFitting(
        PackageFittingId id,
        PackageTypeId typeId,
        PackageMaterialId materialId,
        string fittingImageUrl)
    {
        Id = id;
        TypeId = typeId;
        MaterialId = materialId;
        FittingImageUrl = fittingImageUrl;
    }

    public static PackageFitting New(
        PackageTypeId typeId,
        PackageMaterialId materialId,
        string fittingImageUrl) =>
        new(PackageFittingId.New(), typeId, materialId, fittingImageUrl);

    public void Update(
        PackageTypeId typeId,
        PackageMaterialId materialId,
        string fittingImageUrl)
    {
        TypeId = typeId;
        Type = null;
        MaterialId = materialId;
        Material = null;
        FittingImageUrl = fittingImageUrl;
    }
}
