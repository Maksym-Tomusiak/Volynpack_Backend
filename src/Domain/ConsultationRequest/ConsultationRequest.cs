namespace Domain.ConsultationRequest;

public class ConsultationRequest
{
    public ConsultationRequestId Id { get; private set; }
    public string PhoneNumber { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public bool IsActive { get; private set; }

    private ConsultationRequest(ConsultationRequestId id, string phoneNumber, DateTime createdAt, bool isActive)
    {
        Id = id;
        PhoneNumber = phoneNumber;
        CreatedAt = createdAt;
        IsActive = isActive;
    }

    public static ConsultationRequest New(string phoneNumber) => 
        new(ConsultationRequestId.New(), phoneNumber, DateTime.UtcNow, true);

    public void Update(string phoneNumber, bool isActive)
    {
        PhoneNumber = phoneNumber;
        IsActive = isActive;
    }
}