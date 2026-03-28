namespace Application.Common.Interfaces.Services;

public interface IEmailTemplateService
{
    Task<string> GetTemplateAsync(string templateName, Dictionary<string, string> placeholders);
}