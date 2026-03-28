using Application.NewsCategories.Exceptions;

namespace Api.Modules.Errors;

public static class NewsCategoryErrorHandler
{
    public static IResult ToIResult(this NewsCategoryException exception)
    {
        return exception switch
        {
            NewsCategoryNotFoundException => Results.Problem(
                detail: exception.Message,
                statusCode: StatusCodes.Status404NotFound),

            NewsCategoryAlreadyExistsException => Results.Problem(
                detail: exception.Message,
                statusCode: StatusCodes.Status409Conflict),

            NewsCategoryUnknownException => Results.Problem(
                detail: exception.Message,
                statusCode: StatusCodes.Status500InternalServerError),

            _ => Results.Problem(
                detail: "NewsCategory error handler is not implemented",
                statusCode: StatusCodes.Status500InternalServerError)
        };
    }
}

