namespace Domain.ConsultationRequest;

public record ConsultationRequestId(Guid Value)
{
    public static ConsultationRequestId Empty() => new(Guid.Empty);
    public static ConsultationRequestId New() => new(Guid.NewGuid());
    public override string ToString() => Value.ToString();
}