using FluentValidation;
using WHMS.Application.Features.Queries.Employee.GetAllEmployees;

namespace WHMS.Application.Validators.Employee;

public class GetAllEmployeesQueryValidator : AbstractValidator<GetAllEmployeesQueryRequest>
{
    public GetAllEmployeesQueryValidator()
    {
        RuleFor(x => x.WarehouseId)
            .Must(x => string.IsNullOrWhiteSpace(x) || Guid.TryParse(x, out _))
            .WithMessage("The WarehouseId must be a valid GUID or empty.");

        RuleFor(x => x.CityId)
            .Must(x => string.IsNullOrWhiteSpace(x) || Guid.TryParse(x, out _))
            .WithMessage("The CityId must be a valid GUID or empty.");
        
        RuleFor(x => x.DistrictId)
            .Must(x => string.IsNullOrWhiteSpace(x) || Guid.TryParse(x, out _))
            .WithMessage("The DistrictId must be a valid GUID or empty.");
        
        RuleFor(x => x.NeighborhoodId)
            .Must(x => string.IsNullOrWhiteSpace(x) || Guid.TryParse(x, out _))
            .WithMessage("The NeighborhoodId must be a valid GUID or empty.");
        
        RuleFor(x => x.Page)
            .GreaterThan(0).WithMessage("Page number must be greater than 0.");

        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("Page size must be a greater than 0.");
    }
}