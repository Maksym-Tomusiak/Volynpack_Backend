using Domain.ConsultationRequest;

namespace Application.Common.Interfaces.Repositories;

public interface IConsultationRequestRepository
{
    Task<ConsultationRequest> Add(ConsultationRequest request, CancellationToken cancellationToken);
    Task<ConsultationRequest> Update(ConsultationRequest request, CancellationToken cancellationToken);
    Task<ConsultationRequest> Delete(ConsultationRequest request, CancellationToken cancellationToken);
}