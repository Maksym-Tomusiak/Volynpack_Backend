using Api.Dtos;
using FluentValidation;

namespace Api.Modules.Validators;

public class CreateOrderDtoValidator : AbstractValidator<CreateOrderDto>
{
    public CreateOrderDtoValidator()
    {
        RuleFor(x => x.Items).NotEmpty();
        RuleForEach(x => x.Items).SetValidator(new CreateOrderItemDtoValidator());
        RuleFor(x => x.DeliveryMethodId).NotEmpty();
        
        RuleFor(x => x.FullName)
            .NotEmpty()
            .MaximumLength(200);
            
        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .MaximumLength(20);
            
        RuleFor(x => x.Town)
            .MaximumLength(100);
        
        RuleFor(x => x.Branch)
            .MaximumLength(100);
    }
}

public class CreateOrderItemDtoValidator : AbstractValidator<CreateOrderItemDto>
{
    public CreateOrderItemDtoValidator()
    {
        RuleFor(x => x.ProductVariantId).NotEmpty();
        RuleFor(x => x.Quantity).GreaterThan(0);
        RuleFor(x => x.PrintingOptionId).NotEmpty();
    }
}

public class UpdateOrderDtoValidator : AbstractValidator<UpdateOrderDto>
{
    public UpdateOrderDtoValidator()
    {
        RuleFor(x => x.OrderStatusId).NotEmpty();
        RuleFor(x => x.DeliveryMethodId).NotEmpty();
        
        RuleFor(x => x.FullName)
            .NotEmpty()
            .MaximumLength(200);
            
        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .MaximumLength(20);
            
        RuleFor(x => x.Town)
            .MaximumLength(100);
            
        RuleFor(x => x.Branch)
            .MaximumLength(100);
    }
}
