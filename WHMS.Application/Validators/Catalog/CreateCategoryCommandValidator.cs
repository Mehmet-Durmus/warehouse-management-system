using FluentValidation;
using WHMS.Application.Features.Command.Catalog;

namespace WHMS.Application.Validators.Catalog;

public class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommandRequest>
{
    public CreateCategoryCommandValidator()
    {
        RuleFor(x => x.CategoryName)
            .NotEmpty().WithMessage("Category name is required.")
            .MaximumLength(50).WithMessage("The category name must be no more than 50 characters long.");
    }
}