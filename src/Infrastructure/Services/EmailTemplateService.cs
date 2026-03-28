using Application.Common.Interfaces.Services;
using Microsoft.AspNetCore.Hosting;

namespace Infrastructure.Services;

public class EmailTemplateService(IWebHostEnvironment env) : IEmailTemplateService
{
    private readonly string _templatePath = Path.Combine(env.ContentRootPath, "EmailTemplates");

    public async Task<string> GetTemplateAsync(string templateName, Dictionary<string, string> placeholders)
    {
        var filePath = Path.Combine(_templatePath, $"{templateName}.html");
        var template = await File.ReadAllTextAsync(filePath);

        foreach (var placeholder in placeholders)
        {
            template = template.Replace($"{{{{{placeholder.Key}}}}}", placeholder.Value);
        }

        return template;
    }
}