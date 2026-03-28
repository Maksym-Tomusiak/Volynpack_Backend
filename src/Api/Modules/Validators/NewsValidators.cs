using Api.Dtos;
using FluentValidation;

namespace Api.Modules.Validators;

public class NewsCategoryCreateDtoValidator : AbstractValidator<NewsCategoryCreateDto>
{
    public NewsCategoryCreateDtoValidator()
    {
        RuleFor(x => x.TitleUk).NotEmpty().MaximumLength(200);
        RuleFor(x => x.TitleEn).NotEmpty().MaximumLength(200);
    }
}

public class HashtagCreateDtoValidator : AbstractValidator<HashtagCreateDto>
{
    public HashtagCreateDtoValidator()
    {
        RuleFor(x => x.NameUk).NotEmpty().MaximumLength(100);
        RuleFor(x => x.NameEn).NotEmpty().MaximumLength(100);
    }
}

public class NewsSectionCreateDtoValidator : AbstractValidator<NewsSectionCreateDto>
{
    public NewsSectionCreateDtoValidator()
    {
        RuleFor(x => x.TitleUk).NotEmpty().MaximumLength(500);
        RuleFor(x => x.TitleEn).NotEmpty().MaximumLength(500);
        RuleFor(x => x.ContentUk).NotEmpty();
        RuleFor(x => x.ContentEn).NotEmpty();
        RuleFor(x => x.Order).GreaterThanOrEqualTo(0);
    }
}

public class NewsCreateDtoValidator : AbstractValidator<NewsCreateDto>
{
    public NewsCreateDtoValidator()
    {
        RuleFor(x => x.TitleUk).NotEmpty().MaximumLength(500);
        RuleFor(x => x.TitleEn).NotEmpty().MaximumLength(500);
        RuleFor(x => x.SeoUrlUk).NotEmpty().MaximumLength(500);
        RuleFor(x => x.SeoUrlEn).NotEmpty().MaximumLength(500);
        RuleFor(x => x.Photo).NotNull();
        RuleFor(x => x.PrefaceUk).NotEmpty();
        RuleFor(x => x.PrefaceEn).NotEmpty();
        RuleFor(x => x.CategoryId).NotEmpty();
        
        RuleForEach(x => x.Sections).SetValidator(new NewsSectionCreateDtoValidator());
    }
}

public class NewsUpdateDtoValidator : AbstractValidator<NewsUpdateDto>
{
    public NewsUpdateDtoValidator()
    {
        RuleFor(x => x.TitleUk).NotEmpty().MaximumLength(500);
        RuleFor(x => x.TitleEn).NotEmpty().MaximumLength(500);
        RuleFor(x => x.SeoUrlUk).NotEmpty().MaximumLength(500);
        RuleFor(x => x.SeoUrlEn).NotEmpty().MaximumLength(500);
        RuleFor(x => x.PrefaceUk).NotEmpty();
        RuleFor(x => x.PrefaceEn).NotEmpty();
        RuleFor(x => x.CategoryId).NotEmpty();
        
        RuleForEach(x => x.Sections).SetValidator(new NewsSectionCreateDtoValidator());
    }
}
