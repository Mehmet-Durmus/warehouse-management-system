using FluentValidation;
using WHMS.Application.Features.Command.Warehouse.DeleteWarehouse;

namespace WHMS.Application.Validators.Employee;

public class DeleteWarehouseCommandValidator : AbstractValidator<DeleteWarehouseCommandRequest>
{
    public DeleteWarehouseCommandValidator()
    {
        RuleFor(x => x.WarehouseId)
            .NotEmpty().WithMessage("WarehouseId is required")
            .Must(x => Guid.TryParse(x, out _)).WithMessage("WarehouseId must be a valid GUID.");
    }
}