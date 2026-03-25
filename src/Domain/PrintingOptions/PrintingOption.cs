namespace Domain.PrintingOptions;

public class PrintingOption
{
    public PrintingOptionId Id { get; private set; }
    
    // Бачить покупець, тому двомовне
    public LocalizedString Name { get; private set; }

    private PrintingOption(PrintingOptionId id, LocalizedString name)
    {
        Id = id;
        Name = name;
    }

    public static PrintingOption New(LocalizedString name) => new(PrintingOptionId.New(), name);

    public void Update(LocalizedString name)
    {
        Name = name;
    }
}