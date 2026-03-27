using FluentValidation;
using WHMS.Application.Features.Command.Warehouse.AssignEmployees;

namespace WHMS.Application.Validators.Warehose;

public class AssignEmployeesCommandValidator : AbstractValidator<AssignEmployeesCommandRequest>
{
    public AssignEmployeesCommandValidator()
    {
        RuleFor(x => x.WarehouseId)
            .Must(x => Guid.TryParse(x, out _))
            .WithMessage("The WarehouseId must be a valid GUID.");

        RuleFor(x => x.ManagerId)
            .Must(x => Guid.TryParse(x, out _))
            .WithMessage("The ManagerId must be a valid GUID.");
        
        RuleFor(x => x.StaffIds)
            .ForEach(x => x.Must(s => Guid.TryParse(s, out _)).WithMessage("StaffId must be a valid GUID."));
            
    }
}