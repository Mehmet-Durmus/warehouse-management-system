using FluentValidation;
using WHMS.Application.Features.Queries.Store.GetStore;

namespace WHMS.Application.Validators.Store;

public class GetStoreQueryValidator : AbstractValidator<GetStoreQueryRequest>
{
    public GetStoreQueryValidator()
    {
        RuleFor(x => x.StoreId)
            .NotEmpty().WithMessage("The StoreId is required.")
            .Must(x => Guid.TryParse(x, out _)).WithMessage("The StoreId must be a valid GUID.");
    }
}