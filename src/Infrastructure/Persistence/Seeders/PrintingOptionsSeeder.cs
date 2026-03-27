using Domain;
using Domain.PrintingOptions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Persistence.Seeders;

public static class PrintingOptionsSeeder
{
    public static async Task SeedPrintingOptionsAsync(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        await using var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var options = new List<PrintingOption>([
            PrintingOption.New(new LocalizedString("Без друку", "No print"), 0),
            PrintingOption.New(new LocalizedString("1-3 кольори (Логотип)", "1-3 Colors (Logo)"), 15),
            PrintingOption.New(new LocalizedString("Повноколірний друк (Патерн/Заливка)", "Full Color Print (Pattern/Fill)"), 25)
        ]);

        if (context.PrintingOptions.Any())
        {
            return;
        }
        
        foreach (var printingOption in options)
        {
            await context.PrintingOptions.AddAsync(printingOption);
        }
        
        await context.SaveChangesAsync();
    }

}