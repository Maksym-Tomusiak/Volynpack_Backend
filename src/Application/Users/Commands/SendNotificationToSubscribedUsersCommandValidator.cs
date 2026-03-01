using FluentValidation;

namespace Application.Users.Commands;

public class SendNotificationToSubscribedUsersCommandValidator : AbstractValidator<SendNotificationToSubscribedUsersCommand>
{
    public SendNotificationToSubscribedUsersCommandValidator()
    {
        RuleFor(x => x.Message).NotEmpty();
    }
}