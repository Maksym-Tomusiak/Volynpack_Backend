using FluentValidation;

namespace Application.Users.Commands;

public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.NewPassword).MinimumLength(5).MaximumLength(255);
        RuleFor(x => x.NewUsername).MinimumLength(5).MaximumLength(255);
        RuleFor(x => x.NewEmail).EmailAddress().MaximumLength(255);
        RuleFor(x => x.RoleId).NotEmpty();
    }
}