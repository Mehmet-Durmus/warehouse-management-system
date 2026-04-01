using FluentValidation;
using WHMS.Application.Features.Command.WasteRecord.CreateWasteRecord;

namespace WHMS.Application.Validators.WasteRecord;

public class CreateWasteRecordCommandValidator : AbstractValidator<CreateWasteRecordCommandRequest>
{
    public CreateWasteRecordCommandValidator()
    {       
        RuleFor(x => x.SkuId)
            .NotEmpty().WithMessage("The SkuId is required.")
            .Must(x => Guid.TryParse(x, out _)).WithMessage("The SkuId must be a valid GUID.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Waste record quantity must be greater than 0.");

        RuleFor(x => x.Description)
            .Matches(@"\b\p{L}{2,}\b")
            .WithMessage("Description must contain at least one word with minimum 2 letters.");
    }
}