using Application.PackageFittings.Exceptions;

namespace Api.Modules.Errors;

public static class PackageFittingErrorHandler
{
    public static IResult ToIResult(this PackageFittingException exception)
    {
        return exception switch
        {
            PackageFittingNotFoundException => Results.Problem(
                detail: exception.Message,
                statusCode: StatusCodes.Status404NotFound),

            PackageFittingAlreadyExistsException => Results.Problem(
                detail: exception.Message,
                statusCode: StatusCodes.Status409Conflict),

            PackageFittingUnknownException => Results.Problem(
                detail: exception.Message,
                statusCode: StatusCodes.Status500InternalServerError),

            _ => Results.Problem(
                detail: "PackageFitting error handler is not implemented",
                statusCode: StatusCodes.Status500InternalServerError)
        };
    }
}
