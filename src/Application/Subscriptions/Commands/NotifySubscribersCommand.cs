using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Services;
using Application.Common.Interfaces.Services.BackgroundEmail;
using Application.Common.Models;
using Microsoft.Extensions.Options;

namespace Application.Subscriptions.Commands;

public record NotifySubscribersCommand(string Message);

public class NotifySubscribersCommandHandler(
    ISubscriptionQueries subscriptionQueries,
    IEmailTemplateService templateService,
    IBackgroundEmailQueue emailQueue,
    IOptions<EmailSettings> emailSettings)
{
    public async Task Handle(NotifySubscribersCommand command, CancellationToken cancellationToken)
    {
        var subscribers = await subscriptionQueries.GetAll(cancellationToken);
        var settings = emailSettings.Value;

        foreach (var sub in subscribers)
        {
            var unsubscribeLink = $"{settings.BaseApiUrl}/api/subscriptions/unsubscribe/{sub.UnsubscribeToken}";
            
            var placeholders = new Dictionary<string, string>
            {
                { "Message", command.Message },
                { "UnsubscribeLink", unsubscribeLink }
            };

            var htmlBody = await templateService.GetTemplateAsync("NotificationEmailTemplate", placeholders);

            var message = new EmailMessage(
                ToEmail: sub.Email,
                Subject: "Нове оновлення від Volynpack",
                Body: htmlBody,
                IsHtml: true,
                UnsubscribeUrl: unsubscribeLink,
                IsSubscribe: true
            );

            await emailQueue.QueueEmail(message);
        }
    }
}
