using Domain;
using Domain.DelivaryMethods;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Persistence.Seeders;

public static class DeliveryMethodsSeeder
{
    public static async Task SeedDeliveryMethodsAsync(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        await using var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var methods = new List<DeliveryMethod>([
            DeliveryMethod.New(new LocalizedString("Доставка Новою Поштою", "Nova Poshta delivery methods")),
            DeliveryMethod.New(new LocalizedString("Самовивіз", "Self-delivery"))
        ]);

        if (context.DeliveryMethods.Any())
        {
            return;
        }
        
        foreach (var method in methods)
        {
            await context.DeliveryMethods.AddAsync(method);
        }
        
        await context.SaveChangesAsync();
    }

}