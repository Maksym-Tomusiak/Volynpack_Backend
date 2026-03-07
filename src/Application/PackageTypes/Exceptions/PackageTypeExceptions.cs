namespace Application.PackageTypes.Exceptions;

public class PackageTypeException(Guid id, string message, Exception? innerException = null)
    : Exception(message, innerException)
{
    public Guid Id { get; } = id;
}

public class PackageTypeNotFoundException(Guid id)
    : PackageTypeException(id, $"PackageType under id: {id} not found!");

public class PackageTypeAlreadyExistsException(Guid id)
    : PackageTypeException(id, $"PackageType under id: {id} already exists!");

public class PackageTypeUnknownException(Guid id, Exception innerException)
    : PackageTypeException(id, $"Unknown exception for PackageType under id: {id}!", innerException);
