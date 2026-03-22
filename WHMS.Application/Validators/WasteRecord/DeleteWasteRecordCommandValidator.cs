using FluentValidation;
using WHMS.Application.Features.Command.WasteRecord.DeleteWasteRecord;

namespace WHMS.Application.Validators.WasteRecord;

public class DeleteWasteRecordCommandValidator : AbstractValidator<DeleteWasteRecordCommandRequest>
{
    public DeleteWasteRecordCommandValidator()
    {
        RuleFor(x => x.WasteRecordId)
            .NotNull().WithMessage("The WasteRecordId is required.")
            .Must(x => Guid.TryParse(x, out _)).WithMessage("The WasteRecordId must be a valid GUID.");
    }
}