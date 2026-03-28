using Application.Orders.Exceptions;

namespace Api.Modules.Errors;

public static class OrderErrorHandler
{
    public static IResult ToIResult(this OrderException exception)
    {
        return exception switch
        {
            OrderNotFoundException or
                OrderStatusNotFoundException or
                ProductVariantNotFoundException or
                PrintingOptionNotFoundException
                => Results.Problem(
                    detail: exception.Message,
                    statusCode: StatusCodes.Status404NotFound),

            OrderUnknownException => Results.Problem(
                detail: exception.Message,
                statusCode: StatusCodes.Status500InternalServerError),

            _ => Results.Problem(
                detail: "Order error handler is not implemented",
                statusCode: StatusCodes.Status500InternalServerError)
        };
    }
}