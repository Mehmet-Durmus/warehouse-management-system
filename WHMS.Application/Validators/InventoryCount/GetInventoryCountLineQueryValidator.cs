using FluentValidation;
using WHMS.Application.Features.Queries.InventoryCount.GetInventoryCountLines;

namespace WHMS.Application.Validators.InventoryCount;

public class GetInventoryCountLineQueryValidator : AbstractValidator<GetInventoryCountLinesQueryRequest>
{
    public GetInventoryCountLineQueryValidator()
    {
        RuleFor(x => x.InventoryCountId)
            .Must(x =>
                string.IsNullOrWhiteSpace(x) ||
                Guid.TryParse(x, out _))
            .WithMessage("The InventoryCountId must be a valid GUID or empty.");

        RuleFor(x => x.SkuId)
            .Must(x =>
                string.IsNullOrWhiteSpace(x) ||
                Guid.TryParse(x, out _))
            .WithMessage("The SkuId must be a valid GUID or empty.");

        RuleFor(x => x.MaxQuantity)
            .GreaterThan(0).WithMessage("Maximum quantity value must be greater than 0.");

        RuleFor(x => x.MinQuantity)
            .GreaterThan(0).WithMessage("Minimum quantity value must be a greater than 0.");
        
        RuleFor(x => x.Page)
            .GreaterThan(0).WithMessage("Page number must be greater than 0.");

        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("Page size must be a greater than 0.");
    }
}