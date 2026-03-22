using FluentValidation;
using WHMS.Application.Features.Queries.WasteRecord.GetWasteRecords;

namespace WHMS.Application.Validators;

public class GetWasteRecordsQueryValidator : AbstractValidator<GetWasteRecordsQueryRequest>
{
    public GetWasteRecordsQueryValidator()
    {
        RuleFor(x => x.WarehouseId)
            .Must(x => 
                string.IsNullOrWhiteSpace(x) ||
                Guid.TryParse(x, out _))
            .WithMessage("The WarehouseId must be a valid GUID or empty.");
        
        RuleFor(x => x.SkuId)
            .Must(x => 
                string.IsNullOrWhiteSpace(x) ||
                Guid.TryParse(x, out _))
            .WithMessage("The SkuId must be a valid GUID or empty.");

        RuleFor(x => x.MinQuantity)
            .GreaterThan(0).WithMessage("Minimum quantity value must be greater than 0.");

        RuleFor(x => x.MaxQuantity)
            .GreaterThan(0).WithMessage("Maximum quantity value must be greater than 0.");
    
        RuleFor(x => x.Page)
            .GreaterThan(0).WithMessage("Page value must be greater than 0.");
        
        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("Page size value must be greater than 0.");
    }
}