using Application.Common.Interfaces.Services;
using Application.Common.Interfaces.Services.BackgroundEmail;
using Hangfire;
using Hangfire.PostgreSql;
using Infrastructure.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services;

public static class ConfigureServices
{
    public static void AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        AddHangfire(services, configuration);

        AddEmailService(services, configuration);

        AddFileService(services, configuration);
    }
    
    public static void SetupStaticFiles(this WebApplication app)
    {
        var fileSettings = app.Services.GetRequiredService<IOptions<FileStorageSettings>>().Value;
        var physicalPath = fileSettings.UploadPath;
        if (!Directory.Exists(physicalPath))
        {
            Directory.CreateDirectory(physicalPath);
        }
        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(physicalPath),
            RequestPath = "/uploads" 
        });
    }

    private static void AddHangfire(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("HangfireConnection");
        services.AddHangfire(config =>
            config.UsePostgreSqlStorage((opt) => { opt.UseNpgsqlConnection(connectionString); }));
        services.AddHangfireServer();
    }
    private static void AddFileService(IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<FileStorageSettings>(
            configuration.GetSection("FileStorageSettings")
        );
        services.AddScoped<IFileService, FileService>();
    }

    private static void AddEmailService(IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
        services.AddScoped<IEmailService, EmailService>();

        services.AddSingleton<IBackgroundEmailQueue, BackgroundEmailQueue>();
        services.AddHostedService<EmailBackgroundService>();
    }
}