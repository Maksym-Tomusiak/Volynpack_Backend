using FluentValidation;

namespace Application.Users.Commands;

public class SubscribeEmailCommandValidator : AbstractValidator<SubscribeEmailCommand>
{
    public SubscribeEmailCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
    }
}