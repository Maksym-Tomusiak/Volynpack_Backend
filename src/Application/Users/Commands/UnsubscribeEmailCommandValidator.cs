using FluentValidation;

namespace Application.Users.Commands;

public class UnsubscribeEmailCommandValidator : AbstractValidator<UnsubscribeEmailCommand>
{
    public UnsubscribeEmailCommandValidator()
    {
        RuleFor(x => x.Token).NotEmpty();
    }
}