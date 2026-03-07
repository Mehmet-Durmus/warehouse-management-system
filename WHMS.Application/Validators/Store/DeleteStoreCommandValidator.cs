using FluentValidation;
using WHMS.Application.Features.Command.Store.DeleteStore;

namespace WHMS.Application.Validators.Store;

public class DeleteStoreCommandValidator : AbstractValidator<DeleteStoreCommandRequest>
{
    public DeleteStoreCommandValidator()
    {
        RuleFor(x => x.StoreId)
            .NotEmpty().WithMessage("The StoreId is required.")
            .Must(x => Guid.TryParse(x, out _)).WithMessage("The StoreId must be a valid GUID.");
    }
}