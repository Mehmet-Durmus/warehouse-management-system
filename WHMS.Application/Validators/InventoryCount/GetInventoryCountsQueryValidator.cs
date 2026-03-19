using FluentValidation;
using WHMS.Application.Features.Queries.InventoryCount.GetInventoryCounts;

namespace WHMS.Application.Validators.InventoryCount;

public class GetInventoryCountsQueryValidator : AbstractValidator<GetInventoryCountsQueryRequest>
{
    public GetInventoryCountsQueryValidator()
    {
        RuleFor(x => x.WarehouseId)
            .Must(x => 
                string.IsNullOrWhiteSpace(x) ||
                Guid.TryParse(x, out _))
            .WithMessage("The WarehouseId must be a valid GUID or empty.");
        
        RuleFor(x => x.Page)
            .GreaterThan(0).WithMessage("Page number must be greater than 0.");

        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("Page size must be a greater than 0.");
    }
}