using Application.PackageMaterials.Exceptions;

namespace Api.Modules.Errors;

public static class PackageMaterialErrorHandler
{
    public static IResult ToIResult(this PackageMaterialException exception)
    {
        return exception switch
        {
            PackageMaterialNotFoundException => Results.Problem(
                detail: exception.Message,
                statusCode: StatusCodes.Status404NotFound),

            PackageMaterialAlreadyExistsException => Results.Problem(
                detail: exception.Message,
                statusCode: StatusCodes.Status409Conflict),

            PackageMaterialUnknownException => Results.Problem(
                detail: exception.Message,
                statusCode: StatusCodes.Status500InternalServerError),

            _ => Results.Problem(
                detail: "PackageMaterial error handler is not implemented",
                statusCode: StatusCodes.Status500InternalServerError)
        };
    }
}
