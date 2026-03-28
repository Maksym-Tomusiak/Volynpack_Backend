namespace Application.ProductCategories.Exceptions;

public class ProductCategoryException(Guid id, string message, Exception? innerException = null)
    : Exception(message, innerException)
{
    public Guid Id { get; } = id;
}

public class ProductCategoryNotFoundException(Guid id)
    : ProductCategoryException(id, $"ProductCategory under id: {id} not found!");

public class ProductCategoryUnknownException(Guid id, Exception innerException)
    : ProductCategoryException(id, $"Unknown exception for ProductCategory under id: {id}!", innerException);