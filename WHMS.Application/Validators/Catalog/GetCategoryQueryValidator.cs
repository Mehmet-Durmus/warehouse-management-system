using FluentValidation;
using WHMS.Application.Features.Queries.Catalog.GetCategory;

namespace WHMS.Application.Validators.Catalog;

public class GetCategoryQueryValidator : AbstractValidator<GetCategoryQueryRequest>
{
    public GetCategoryQueryValidator()
    {
        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage("The CategoryId is required.")
            .Must(x => Guid.TryParse(x, out _)).WithMessage("The CategoryId must be a valid GUID.");
    }
}