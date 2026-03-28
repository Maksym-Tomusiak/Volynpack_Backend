namespace Application.ProductPhotos.Exceptions;

public class ProductPhotoException(Guid id, string message, Exception? innerException = null)
    : Exception(message, innerException)
{
    public Guid Id { get; } = id;
}

public class ProductPhotoNotFoundException(Guid id)
    : ProductPhotoException(id, $"ProductPhoto under id: {id} not found!");

public class ProductVariantNotFoundException(Guid id)
    : ProductPhotoException(id, $"ProductVariant under id: {id} not found! Cannot attach photo.");

public class ProductPhotoUnknownException(Guid id, Exception innerException)
    : ProductPhotoException(id, $"Unknown exception for ProductPhoto under id: {id}!", innerException);