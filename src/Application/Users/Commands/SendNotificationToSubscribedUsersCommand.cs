using Application.Common.Interfaces.Services;
using Application.Common.Interfaces.Services.BackgroundEmail;
using Application.Users.Exceptions;
using Domain.Users;
using LanguageExt;

namespace Application.Users.Commands;

public record SendNotificationToSubscribedUsersCommand(
    string Message);

public class SendNotificationToSubscribedUsersCommandHandler
{
    public static async Task<Either<UserException, Unit>> Handle(
        SendNotificationToSubscribedUsersCommand command,
        IEmailTemplateService templateService,
        IBackgroundEmailQueue emailQueue,
        CancellationToken cancellationToken)
    {
        var notifiedUsers = new List<User>([new User()
        {
            Email = "maxtomusiak315@gmail.com"
        }]);
        var unsubscribeToken = Guid.NewGuid();
        
        foreach (var user in notifiedUsers)
        {
            var placeholders = new Dictionary<string, string>
            {
                { "Message", command.Message },
                { "UnsubscribeLink", $"https://localhost:7155/users/unsubscribe?token={unsubscribeToken}" }
            };

            string htmlBody = await templateService.GetTemplateAsync("NotificationTemplate", placeholders);

            var message = new EmailMessage(
                ToEmail: user.Email,
                Subject: "Важливе сповіщення",
                Body: htmlBody,
                IsHtml: true,
                UnsubscribeUrl: $"https://luxsat.net/unsubscribe?token={unsubscribeToken}",
                IsSubscribe: true
            );

            await emailQueue.QueueEmail(message);
        }
        
        return Unit.Default;
    }
}