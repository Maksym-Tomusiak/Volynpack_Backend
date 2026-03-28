using Domain.OrderStatuses;
using Domain.Roles;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Persistence.Seeders;

public static class OrderStatusesSeeder
{
    public static async Task SeedOrderStatusesAsync(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        await using var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var statuses = new[]
        {
            "Нове",
            "Обробляється",
            "Оброблене",
            "Скасоване"
        };

        foreach (var status in statuses)
        {
            await context.OrderStatuses.AddAsync(OrderStatus.New(status));
        }

        if (context.OrderStatuses.Any())
        {
            return;
        }

        
        await context.SaveChangesAsync();
    }

}