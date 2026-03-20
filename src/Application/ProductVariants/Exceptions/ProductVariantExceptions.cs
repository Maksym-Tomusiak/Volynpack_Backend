namespace Application.ProductVariants.Exceptions;

public class ProductVariantException(Guid id, string message, Exception? innerException = null)
    : Exception(message, innerException)
{
    public Guid Id { get; } = id;
}

public class ProductVariantNotFoundException(Guid id)
    : ProductVariantException(id, $"ProductVariant under id: {id} not found!");

public class ProductVariantDependencyNotFoundException(Guid id, string dependencyName)
    : ProductVariantException(id, $"Dependency '{dependencyName}' under id: {id} not found! Cannot process ProductVariant.");

public class ProductVariantUnknownException(Guid id, Exception innerException)
    : ProductVariantException(id, $"Unknown exception for ProductVariant under id: {id}!", innerException);