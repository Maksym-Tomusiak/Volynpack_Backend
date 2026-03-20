using Application.ProductPhotos.Exceptions;

namespace Api.Modules.Errors;

public static class ProductPhotoErrorHandler
{
    public static IResult ToIResult(this ProductPhotoException exception)
    {
        return exception switch
        {
            ProductPhotoNotFoundException or
                ProductVariantNotFoundException
                => Results.Problem(
                    detail: exception.Message,
                    statusCode: StatusCodes.Status404NotFound),

            ProductPhotoUnknownException => Results.Problem(
                detail: exception.Message,
                statusCode: StatusCodes.Status500InternalServerError),

            _ => Results.Problem(
                detail: "Product photo error handler is not implemented",
                statusCode: StatusCodes.Status500InternalServerError)
        };
    }
}