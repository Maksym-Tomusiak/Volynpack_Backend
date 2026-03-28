using Application.PackageTypes.Exceptions;

namespace Api.Modules.Errors;

public static class PackageTypeErrorHandler
{
    public static IResult ToIResult(this PackageTypeException exception)
    {
        return exception switch
        {
            PackageTypeNotFoundException => Results.Problem(
                detail: exception.Message,
                statusCode: StatusCodes.Status404NotFound),

            PackageTypeAlreadyExistsException => Results.Problem(
                detail: exception.Message,
                statusCode: StatusCodes.Status409Conflict),

            PackageTypeUnknownException => Results.Problem(
                detail: exception.Message,
                statusCode: StatusCodes.Status500InternalServerError),

            _ => Results.Problem(
                detail: "PackageType error handler is not implemented",
                statusCode: StatusCodes.Status500InternalServerError)
        };
    }
}
