namespace Application.PackageMaterials.Exceptions;

public class PackageMaterialException(Guid id, string message, Exception? innerException = null)
    : Exception(message, innerException)
{
    public Guid Id { get; } = id;
}

public class PackageMaterialNotFoundException(Guid id)
    : PackageMaterialException(id, $"PackageMaterial under id: {id} not found!");

public class PackageMaterialAlreadyExistsException(Guid id)
    : PackageMaterialException(id, $"PackageMaterial under id: {id} already exists!");

public class PackageMaterialUnknownException(Guid id, Exception innerException)
    : PackageMaterialException(id, $"Unknown exception for PackageMaterial under id: {id}!", innerException);
