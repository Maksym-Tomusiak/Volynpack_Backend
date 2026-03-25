using Api.Dtos.Products;
using FluentValidation;

namespace Api.Modules.Validators;

public class ProductTextFeatureInputDtoValidator : AbstractValidator<ProductTextFeatureInputDto>
{
    public ProductTextFeatureInputDtoValidator()
    {
        RuleFor(x => x.TitleUk).NotEmpty().MaximumLength(500);
        RuleFor(x => x.TitleEn).NotEmpty().MaximumLength(500);
        RuleFor(x => x.DescriptionUk).NotEmpty();
        RuleFor(x => x.DescriptionEn).NotEmpty();
    }
}

public class ProductCreateDtoValidator : AbstractValidator<ProductCreateDto>
{
    public ProductCreateDtoValidator()
    {
        RuleFor(x => x.TypeId).NotEmpty();
        RuleFor(x => x.TitleUk).NotEmpty().MaximumLength(500);
        RuleFor(x => x.TitleEn).NotEmpty().MaximumLength(500);
        RuleFor(x => x.DescriptionUk).NotEmpty();
        RuleFor(x => x.DescriptionEn).NotEmpty();
        RuleFor(x => x.CategoryIds).NotEmpty();
        
        RuleForEach(x => x.SuitableFor).SetValidator(new ProductTextFeatureInputDtoValidator());
        RuleForEach(x => x.GeneralCharacteristics).SetValidator(new ProductTextFeatureInputDtoValidator());
    }
}

public class ProductUpdateDtoValidator : AbstractValidator<ProductUpdateDto>
{
    public ProductUpdateDtoValidator()
    {
        RuleFor(x => x.TypeId).NotEmpty();
        RuleFor(x => x.TitleUk).NotEmpty().MaximumLength(500);
        RuleFor(x => x.TitleEn).NotEmpty().MaximumLength(500);
        RuleFor(x => x.DescriptionUk).NotEmpty();
        RuleFor(x => x.DescriptionEn).NotEmpty();
        RuleFor(x => x.CategoryIds).NotEmpty();
        
        RuleForEach(x => x.SuitableFor).SetValidator(new ProductTextFeatureInputDtoValidator());
        RuleForEach(x => x.GeneralCharacteristics).SetValidator(new ProductTextFeatureInputDtoValidator());
    }
}
