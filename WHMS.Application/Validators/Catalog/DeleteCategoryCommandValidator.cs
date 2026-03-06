using FluentValidation;
using WHMS.Application.Features.Command.Catalog.DeleteCategory;

namespace WHMS.Application.Validators.Catalog;

public class DeleteCategoryCommandValidator : AbstractValidator<DeleteCategoryCommandRequest>
{
    public DeleteCategoryCommandValidator()
    {
        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage("The CategoryId is required.")
            .Must(x => Guid.TryParse(x, out _)).WithMessage("The CategoryId must be a valid GUID.");
    }
}