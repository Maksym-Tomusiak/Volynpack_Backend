using System;
using System.Threading.Tasks;
using Application.Common.Interfaces.Services;
using Application.Common.Models;
using Infrastructure.Settings;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Infrastructure.Services;

public class EmailService(IOptions<EmailSettings> emailSettings) : IEmailService
{
    private readonly EmailSettings _settings = emailSettings.Value;

    public async Task SendEmail(string to, string subject, string body, string? unsubscribeUrl = null, bool isHtml = false, bool isSubscribe = false)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_settings.SenderName, _settings.SenderEmail));
        message.To.Add(new MailboxAddress("Recipient", to));
        message.Subject = subject;
        message.Body = new TextPart(isHtml ? "html" : "plain") { Text = body };
        if (isSubscribe && !string.IsNullOrEmpty(unsubscribeUrl))
        {
            // Standard one-click unsubscribe headers (RFC 8058)
            // Including both mailto and https for maximum compatibility
            message.Headers.Add("List-Unsubscribe", $"<{unsubscribeUrl}>, <mailto:{_settings.SenderEmail}?subject=unsubscribe-{unsubscribeUrl.Split('/').Last()}>");
            message.Headers.Add("List-Unsubscribe-Post", "List-Unsubscribe=One-Click");
        }
        try
        {
            using var client = new SmtpClient();
            await client.ConnectAsync(_settings.SmtpHost, _settings.SmtpPort, SecureSocketOptions.SslOnConnect);
            await client.AuthenticateAsync(_settings.SenderEmail, _settings.SenderPassword);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to send email: {ex.Message}");
        }
    }
}