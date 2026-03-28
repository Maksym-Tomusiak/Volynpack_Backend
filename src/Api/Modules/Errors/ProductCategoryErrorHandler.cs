using Application.ProductCategories.Exceptions;

namespace Api.Modules.Errors;

public static class ProductCategoryErrorHandler
{
    public static IResult ToIResult(this ProductCategoryException exception)
    {
        return exception switch
        {
            ProductCategoryNotFoundException
                => Results.Problem(
                    detail: exception.Message,
                    statusCode: StatusCodes.Status404NotFound),

            ProductCategoryUnknownException => Results.Problem(
                detail: exception.Message,
                statusCode: StatusCodes.Status500InternalServerError),

            _ => Results.Problem(
                detail: "Product category error handler is not implemented",
                statusCode: StatusCodes.Status500InternalServerError)
        };
    }
}