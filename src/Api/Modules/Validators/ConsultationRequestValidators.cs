using Api.Dtos;
using FluentValidation;

namespace Api.Modules.Validators;

public class ConsultationRequestCreateDtoValidator : AbstractValidator<ConsultationRequestCreateDto>
{
    public ConsultationRequestCreateDtoValidator()
    {
        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .MaximumLength(20);
    }
}

public class ConsultationRequestUpdateDtoValidator : AbstractValidator<ConsultationRequestUpdateDto>
{
    public ConsultationRequestUpdateDtoValidator()
    {
        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .MaximumLength(20);
            
        RuleFor(x => x.IsActive).NotNull();
    }
}
