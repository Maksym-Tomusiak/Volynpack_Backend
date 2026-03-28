namespace Application.NewsCategories.Exceptions;

public class NewsCategoryException(Guid id, string message, Exception? innerException = null)
    : Exception(message, innerException)
{
    public Guid Id { get; } = id;
}

public class NewsCategoryNotFoundException(Guid id)
    : NewsCategoryException(id, $"NewsCategory under id: {id} not found!");

public class NewsCategoryAlreadyExistsException(Guid id)
    : NewsCategoryException(id, $"NewsCategory under id: {id} already exists!");

public class NewsCategoryUnknownException(Guid id, Exception innerException)
    : NewsCategoryException(id, $"Unknown exception for NewsCategory under id: {id}!", innerException);

