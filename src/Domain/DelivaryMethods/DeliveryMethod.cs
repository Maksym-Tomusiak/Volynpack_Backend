namespace Domain.DelivaryMethods;

public class DeliveryMethod
{
    public DeliveryMethodId Id { get; private set; }
    public LocalizedString Name { get; private set; }

    private DeliveryMethod(DeliveryMethodId id, LocalizedString name)
    {
        Id = id;
        Name = name;
    }

    public static DeliveryMethod New(LocalizedString name) => new(DeliveryMethodId.New(), name);

    public void Update(LocalizedString name)
    {
        Name = name;
    }
}
