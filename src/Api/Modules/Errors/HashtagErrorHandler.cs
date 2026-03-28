using Application.Hashtags.Exceptions;

namespace Api.Modules.Errors;

public static class HashtagErrorHandler
{
    public static IResult ToIResult(this HashtagException exception)
    {
        return exception switch
        {
            HashtagNotFoundException => Results.Problem(
                detail: exception.Message,
                statusCode: StatusCodes.Status404NotFound),

            HashtagAlreadyExistsException => Results.Problem(
                detail: exception.Message,
                statusCode: StatusCodes.Status409Conflict),

            HashtagUnknownException => Results.Problem(
                detail: exception.Message,
                statusCode: StatusCodes.Status500InternalServerError),

            _ => Results.Problem(
                detail: "Hashtag error handler is not implemented",
                statusCode: StatusCodes.Status500InternalServerError)
        };
    }
}

