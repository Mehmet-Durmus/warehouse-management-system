using FluentValidation;
using WHMS.Application.Features.Command.Catalog.UpdateCategory;

namespace WHMS.Application.Validators.Catalog;

public class UpdateCategoryCommandValidator : AbstractValidator<UpdateCategoryCommandRequest>
{
    public UpdateCategoryCommandValidator()
    {
        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage("The CategoryId is required.")
            .Must(x => Guid.TryParse(x, out _)).WithMessage("The CategoryId must be a valid GUID");

        RuleFor(x => x.CategoryName)
            .NotEmpty().WithMessage("The CategoryName is required.")
            .MaximumLength(50).WithMessage("The category name must be no more than 50 characters long.");
    }
}