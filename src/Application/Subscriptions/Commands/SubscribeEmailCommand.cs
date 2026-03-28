using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Common.Interfaces.Services;
using Application.Common.Interfaces.Services.BackgroundEmail;
using Application.Common.Models;
using Domain.Subscriptions;
using LanguageExt;
using Microsoft.Extensions.Options;

namespace Application.Subscriptions.Commands;

public record SubscribeEmailCommand(string Email);

public class SubscribeEmailCommandHandler(
    ISubscriptionQueries subscriptionQueries,
    ISubscriptionRepository subscriptionRepository,
    IEmailTemplateService templateService,
    IBackgroundEmailQueue emailQueue,
    IOptions<EmailSettings> emailSettings)
{
    public async Task<Either<bool, Subscription>> Handle(
        SubscribeEmailCommand command,
        CancellationToken cancellationToken)
    {
        var subscription = await subscriptionQueries.GetByEmail(command.Email, cancellationToken);
        
        return await subscription.Match<Task<Either<bool, Subscription>>>(
            s => Task.FromResult<Either<bool, Subscription>>(false),
            async () =>
            {
                var newSub = Subscription.New(command.Email);
                var result = await subscriptionRepository.Add(newSub, cancellationToken);

                var settings = emailSettings.Value;
                var unsubscribeLink = $"{settings.BaseApiUrl}/api/subscriptions/unsubscribe/{newSub.UnsubscribeToken}";

                var placeholders = new Dictionary<string, string>
                {
                    { "UnsubscribeLink", unsubscribeLink }
                };

                var htmlBody = await templateService.GetTemplateAsync("SubscriptionConfirmationEmailTemplate", placeholders);

                var message = new EmailMessage(
                    ToEmail: newSub.Email,
                    Subject: "Підписка на новини Volynpack підтверджена",
                    Body: htmlBody,
                    IsHtml: true,
                    UnsubscribeUrl: unsubscribeLink,
                    IsSubscribe: true
                );

                await emailQueue.QueueEmail(message);

                return result;
            }
        );
    }
}