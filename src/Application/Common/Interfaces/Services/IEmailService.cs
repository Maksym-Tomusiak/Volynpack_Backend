namespace Application.Common.Interfaces.Services;

public interface IEmailService
{
    Task SendEmail(string to, string subject, string body, string? unsubscribeUrl = null, bool isHtml = false,
        bool isSubscribe = false);
}