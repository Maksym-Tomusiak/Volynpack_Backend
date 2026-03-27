namespace Domain.PrintingOptions;

public class PrintingOption
{
    public PrintingOptionId Id { get; private set; }
    
    public float PriceIncrease { get; private set; }
    
    // Бачить покупець, тому двомовне
    public LocalizedString Name { get; private set; }

    private PrintingOption(PrintingOptionId id, LocalizedString name, float priceIncrease)
    {
        Id = id;
        Name = name;
        PriceIncrease = priceIncrease;
    }

    public static PrintingOption New(LocalizedString name, float priceIncrease) => new(PrintingOptionId.New(), name, priceIncrease);

    public void Update(LocalizedString name, float priceIncrease)
    {
        Name = name;
        PriceIncrease = priceIncrease;
    }
}