namespace Domain.Subscriptions;

public class Subscription
{
    public SubscriptionId Id { get; private set; }
    public string Email { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid UnsubscribeToken { get; set; }
    
    private Subscription(SubscriptionId id, string email, DateTime createdAt, Guid unsubscribeToken)
    {
        Id = id;
        Email = email;
        CreatedAt = createdAt;
        UnsubscribeToken = unsubscribeToken;
    }

    public static Subscription New(string email) =>
        new(SubscriptionId.New(), email, DateTime.UtcNow, Guid.NewGuid());
}
