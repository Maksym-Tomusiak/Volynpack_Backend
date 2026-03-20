namespace Application.Products.Exceptions;

public class ProductException(Guid id, string message, Exception? innerException = null)
    : Exception(message, innerException)
{
    public Guid Id { get; } = id;
}

public class ProductNotFoundException(Guid id)
    : ProductException(id, $"Product under id: {id} not found!");

// Якщо раптом якоїсь із переданих категорій не існує
public class ProductCategoriesNotFoundException(Guid id)
    : ProductException(id, $"One or more ProductCategories were not found for product id: {id}!");

public class ProductUnknownException(Guid id, Exception innerException)
    : ProductException(id, $"Unknown exception for Product under id: {id}!", innerException);