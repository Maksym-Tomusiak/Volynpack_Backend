namespace Application.ConsultationRequests.Exceptions;

public abstract class ConsultationRequestException(string message, Exception? innerException = null) 
    : Exception(message, innerException);

public class ConsultationRequestNotFoundException(Guid id) 
    : ConsultationRequestException($"Consultation request with id {id} was not found.");

public class ConsultationRequestUnknownException(Guid id, Exception innerException) 
    : ConsultationRequestException($"Unknown error occurred while processing consultation request with id {id}.", innerException);