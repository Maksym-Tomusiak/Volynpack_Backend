namespace Application.News.Exceptions;

public class NewsException(Guid id, string message, Exception? innerException = null)
    : Exception(message, innerException)
{
    public Guid Id { get; } = id;
}

public class NewsNotFoundException(Guid id)
    : NewsException(id, $"News under id: {id} not found!");

public class NewsCategoryNotFoundException(Guid id)
    : NewsException(id, $"NewsCategory under id: {id} not found!");

public class NewsHashtagNotFoundException(Guid id)
    : NewsException(id, $"Hashtag under id: {id} not found!");

public class NewsAlreadyExistsException(Guid id)
    : NewsException(id, $"News under id: {id} already exists!");

public class NewsUnknownException(Guid id, Exception innerException)
    : NewsException(id, $"Unknown exception for News under id: {id}!", innerException);

