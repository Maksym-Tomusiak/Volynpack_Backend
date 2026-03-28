using Application.News.Exceptions;

namespace Api.Modules.Errors;

public static class NewsErrorHandler
{
    public static IResult ToIResult(this NewsException exception)
    {
        return exception switch
        {
            NewsNotFoundException or
            NewsCategoryNotFoundException or
            NewsHashtagNotFoundException
                => Results.Problem(
                    detail: exception.Message,
                    statusCode: StatusCodes.Status404NotFound),

            NewsAlreadyExistsException => Results.Problem(
                detail: exception.Message,
                statusCode: StatusCodes.Status409Conflict),

            NewsUnknownException => Results.Problem(
                detail: exception.Message,
                statusCode: StatusCodes.Status500InternalServerError),

            _ => Results.Problem(
                detail: "News error handler is not implemented",
                statusCode: StatusCodes.Status500InternalServerError)
        };
    }
}

