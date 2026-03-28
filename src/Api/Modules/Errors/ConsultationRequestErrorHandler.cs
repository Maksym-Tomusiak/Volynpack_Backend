using Application.ConsultationRequests.Exceptions;

namespace Api.Modules.Errors;

public static class ConsultationRequestErrorHandler
{
    public static IResult ToIResult(this ConsultationRequestException exception)
    {
        return exception switch
        {
            ConsultationRequestNotFoundException => Results.Problem(
                detail: exception.Message,
                statusCode: StatusCodes.Status404NotFound),

            ConsultationRequestUnknownException => Results.Problem(
                detail: exception.Message,
                statusCode: StatusCodes.Status500InternalServerError),

            _ => Results.Problem(
                detail: "Consultation request error handler is not implemented",
                statusCode: StatusCodes.Status500InternalServerError)
        };
    }
}