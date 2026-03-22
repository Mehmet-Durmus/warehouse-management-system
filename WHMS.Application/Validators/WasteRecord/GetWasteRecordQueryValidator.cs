using FluentValidation;
using WHMS.Application.Features.Queries.WasteRecord.GetWasteRecord;

namespace WHMS.Application.Validators.WasteRecord;

public class GetWasteRecordQueryValidator : AbstractValidator<GetWasteRecordQueryRequest>
{
    public GetWasteRecordQueryValidator()
    {
        RuleFor(x => x.WasteRecordId)
            .NotEmpty().WithMessage("The WasteRecordId is required.")
            .Must(x => Guid.TryParse(x, out _)).WithMessage("The WasteRecordId must be a valid GUID.");
    }
}