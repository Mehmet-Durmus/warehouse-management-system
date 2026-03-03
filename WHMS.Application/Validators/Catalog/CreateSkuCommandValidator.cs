using FluentValidation;
using WHMS.Application.Features.Command.Catalog.CreateSku;

namespace WHMS.Application.Validators.Catalog;

public class CreateSkuCommandValidator : AbstractValidator<CreateSkuCommandRequest>
{
    public CreateSkuCommandValidator()
    {
        RuleFor(x => x.SkuName)
            .NotEmpty().WithMessage("The sku name is required.")
            .MaximumLength(50).WithMessage("The sku name must be no more than 50 characters long.");

        RuleFor(x => x.Barcode)
            .NotEmpty().WithMessage("The barcdoe is required.")
            .MaximumLength(100).WithMessage("The barcode must be no more than 100 characters long.");

        RuleFor(x => x.UnitPrice)
            .GreaterThanOrEqualTo(0)
            .LessThanOrEqualTo(999.99m)
            .PrecisionScale(7,2,false)
            .WithMessage("The unit price must be between 99999.99 and 0.");
        
        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage("The category id is required.")
            .Must(x => Guid.TryParse(x, out _)).WithMessage("The CategoryId must be a valid GUID.");
    }
}