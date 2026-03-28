using Application.Products.Exceptions;

namespace Api.Modules.Errors;

public static class ProductErrorHandler
{
    public static IResult ToIResult(this ProductException exception)
    {
        return exception switch
        {
            ProductNotFoundException or
                ProductCategoriesNotFoundException
                => Results.Problem(
                    detail: exception.Message,
                    statusCode: StatusCodes.Status404NotFound),

            ProductUnknownException => Results.Problem(
                detail: exception.Message,
                statusCode: StatusCodes.Status500InternalServerError),

            _ => Results.Problem(
                detail: "Product error handler is not implemented",
                statusCode: StatusCodes.Status500InternalServerError)
        };
    }
}