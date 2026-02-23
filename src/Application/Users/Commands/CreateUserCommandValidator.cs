using FluentValidation;

namespace Application.Users.Commands;

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.Username).NotEmpty().MinimumLength(5).MaximumLength(255);
        RuleFor(x => x.Password).NotEmpty().MinimumLength(5).MaximumLength(255);
        RuleFor(x => x.Email).EmailAddress().MaximumLength(255);
        RuleFor(x => x.RoleId).NotEmpty();
    }   
}