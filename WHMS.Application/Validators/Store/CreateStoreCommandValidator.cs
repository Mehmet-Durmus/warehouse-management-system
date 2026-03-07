using FluentValidation;
using WHMS.Application.Features.Command.Store.CreateStore;

namespace WHMS.Application.Validators.Store;

public class CreateStoreCommandValidator : AbstractValidator<CreateStoreCommandRequest>
{
    public CreateStoreCommandValidator()
    {
        RuleFor(x => x.StoreName)
            .NotEmpty().WithMessage("The store name cannot be empty.")
            .MaximumLength(50).WithMessage("The store name must be no longer than 50 characters long.");
        RuleFor(x => x.CityId)
            .NotEmpty().WithMessage("The CityId is required.")
            .Must(x => Guid.TryParse(x, out _)).WithMessage("The CityId must be a valid GUID.");
        RuleFor(x => x.DistrictId)
            .NotEmpty().WithMessage("The DistrictId is required.")
            .Must(x => Guid.TryParse(x, out _)).WithMessage("The DistrictId must be a valid GUID.");
        RuleFor(x => x.NeighborhoodId)
            .NotEmpty().WithMessage("The NeihgborhoodId is required.")
            .Must(x => Guid.TryParse(x, out _)).WithMessage("The NeighborhoodId must be a valid GUID.");
        RuleFor(x => x.AddressLine)
            .NotEmpty().WithMessage("Address line cannot be empty.");
        RuleFor(x => x.PostalCode)
            .NotEmpty().WithMessage("Postal code cannot be empty.");
    }
}