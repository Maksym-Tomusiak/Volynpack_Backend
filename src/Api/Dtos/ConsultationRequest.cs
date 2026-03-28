using Domain.ConsultationRequest;

namespace Api.Dtos;

public record ConsultationRequestDto(Guid Id, string PhoneNumber, DateTimeOffset CreatedAt, bool IsActive)
{
    public static ConsultationRequestDto FromDomainModel(ConsultationRequest request) =>
        new(request.Id.Value, request.PhoneNumber, request.CreatedAt, request.IsActive);
}

public record ConsultationRequestCreateDto(string PhoneNumber);

public record ConsultationRequestUpdateDto(string PhoneNumber, bool IsActive);