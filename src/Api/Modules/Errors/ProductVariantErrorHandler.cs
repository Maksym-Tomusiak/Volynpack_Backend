using Application.ProductVariants.Exceptions;

namespace Api.Modules.Errors;

public static class ProductVariantErrorHandler
{
    public static IResult ToIResult(this ProductVariantException exception)
    {
        return exception switch
        {
            ProductVariantNotFoundException or
                ProductVariantDependencyNotFoundException
                => Results.Problem(
                    detail: exception.Message,
                    statusCode: StatusCodes.Status404NotFound),

            ProductVariantUnknownException => Results.Problem(
                detail: exception.Message,
                statusCode: StatusCodes.Status500InternalServerError),

            _ => Results.Problem(
                detail: "Product variant error handler is not implemented",
                statusCode: StatusCodes.Status500InternalServerError)
        };
    }
}