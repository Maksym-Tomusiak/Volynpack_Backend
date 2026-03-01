using Application.Users.Exceptions;

namespace Api.Modules.Errors;

public static class UserCategoryErrorHandler
{
    public static IResult ToIResult(this UserException exception)
    {
        return exception switch
        {
            UserNotFoundException or 
            UserIdNotFoundException or
            UserRoleNotFoundException 
            => Results.Problem(
            detail: exception.Message,
            statusCode: StatusCodes.Status404NotFound
            ),
            
            UserWithNameAlreadyExistsException => Results.Problem(
                detail: exception.Message,
                statusCode: StatusCodes.Status409Conflict
            ),
            
            InvalidCredentialsException => Results.Problem(
                detail: exception.Message,
                statusCode: StatusCodes.Status401Unauthorized
            ),
            
            UserUnauthorizedAccessException => Results.Problem(
                detail: exception.Message,
                statusCode: StatusCodes.Status403Forbidden
            ),

            UserUnknownException => Results.Problem(
                detail: exception.Message,
                statusCode: StatusCodes.Status500InternalServerError
            ),

            _ => Results.Problem(
                detail: "UserCategory error handler is not implemented",
                statusCode: StatusCodes.Status500InternalServerError
            )
        };
    }
}

