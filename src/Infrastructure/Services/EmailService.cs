using System;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Application.Common.Interfaces.Services;
using Application.Common.Models;
using Infrastructure.Settings;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using Resend;

namespace Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly EmailSettings _settings;
    private readonly IResend _resend;

    public EmailService(IOptions<EmailSettings> emailSettings)
    {
        _settings = emailSettings.Value;
        
        // Initialize the Resend client with your API key
        _resend = ResendClient.Create(_settings.MailApiKey);
    }

    public async Task SendEmail(string to, string subject, string body, string? unsubscribeUrl = null, bool isHtml = false, bool isSubscribe = false)
    {
        // Use Resend's built-in message model
        var message = new Resend.EmailMessage
        {
            From = $"{_settings.SenderName} <{_settings.SenderEmail}>", // e.g., "Volynpack <info@volynpack.com>"
            To = to,
            Subject = subject,
        };

        // Assign the body based on the isHtml flag
        if (isHtml)
        {
            message.HtmlBody = body;
        }
        else
        {
            message.TextBody = body;
        }

        // Bonus: Resend supports custom headers on the free tier! 
        // We can safely add the standard 1-click unsubscribe headers back.
        if (isSubscribe && !string.IsNullOrEmpty(unsubscribeUrl))
        {
            message.Headers = new Dictionary<string, string>
            {
                { "List-Unsubscribe", $"<{unsubscribeUrl}>" },
                { "List-Unsubscribe-Post", "List-Unsubscribe=One-Click" }
            };
        }

        try
        {
           await _resend.EmailSendAsync(message);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Resend Exception: {ex.Message}");
        }
    }
}