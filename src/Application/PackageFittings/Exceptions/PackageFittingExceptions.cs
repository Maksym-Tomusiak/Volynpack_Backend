namespace Application.PackageFittings.Exceptions;

public class PackageFittingException(Guid id, string message, Exception? innerException = null)
    : Exception(message, innerException)
{
    public Guid Id { get; } = id;
}

public class PackageFittingNotFoundException(Guid id)
    : PackageFittingException(id, $"PackageFitting under id: {id} not found!");

public class PackageFittingAlreadyExistsException(Guid id)
    : PackageFittingException(id, $"PackageFitting under id: {id} already exists!");

public class PackageFittingUnknownException(Guid id, Exception innerException)
    : PackageFittingException(id, $"Unknown exception for PackageFitting under id: {id}!", innerException);
