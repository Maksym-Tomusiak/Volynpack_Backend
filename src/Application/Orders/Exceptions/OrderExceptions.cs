namespace Application.Orders.Exceptions;

public class OrderException(Guid id, string message, Exception? innerException = null)
    : Exception(message, innerException)
{
    public Guid Id { get; } = id;
}

public class OrderNotFoundException(Guid id)
    : OrderException(id, $"Order under id: {id} not found!");

public class ProductVariantNotFoundException(Guid id)
    : OrderException(id, $"ProductVariant under id: {id} not found!");

public class OrderStatusNotFoundException(Guid id)
    : OrderException(id, $"OrderStatus under id: {id} not found!");

public class PrintingOptionNotFoundException(Guid id)
    : OrderException(id, $"PrintingOption under id: {id} not found!");

public class OrderUnknownException(Guid id, Exception innerException)
    : OrderException(id, $"Unknown exception for Order under id: {id}!", innerException);