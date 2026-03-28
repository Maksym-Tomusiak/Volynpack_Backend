namespace Application.Hashtags.Exceptions;

public class HashtagException(Guid id, string message, Exception? innerException = null)
    : Exception(message, innerException)
{
    public Guid Id { get; } = id;
}

public class HashtagNotFoundException(Guid id)
    : HashtagException(id, $"Hashtag under id: {id} not found!");

public class HashtagAlreadyExistsException(Guid id)
    : HashtagException(id, $"Hashtag under id: {id} already exists!");

public class HashtagUnknownException(Guid id, Exception innerException)
    : HashtagException(id, $"Unknown exception for Hashtag under id: {id}!", innerException);

