using Domain.ProductPhotos;

namespace Api.Dtos.ProductPhotos;

public record ProductPhotoDto(Guid Id, Guid ProductVariantId, string PhotoUrl, bool IsPrimary)
{
    public static ProductPhotoDto FromDomainModel(ProductPhoto photo) =>
        new(photo.Id.Value, photo.ProductVariantId.Value, photo.PhotoUrl, photo.IsPrimary);
}

// Використовуємо IFormFile для завантаження картинки
public record ProductPhotoCreateDto(Guid ProductVariantId, IFormFile Photo, bool IsPrimary);

// При оновленні ми змінюємо лише статус головного фото (саму картинку легше перестворити)
public record ProductPhotoUpdateDto(bool IsPrimary);