namespace Domain.ProductPhotos;

public record ProductPhotoId(Guid Value)
{
    public static ProductPhotoId Empty() => new(Guid.Empty);
    public static ProductPhotoId New() => new(Guid.NewGuid());
    public override string ToString() => Value.ToString();
}