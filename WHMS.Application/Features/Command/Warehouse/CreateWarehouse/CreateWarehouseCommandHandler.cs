using System.Globalization;
using MediatR;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Application.Common.Constants;
using WHMS.Domain.BusinessRules.Shared;
using WHMS.Domain.BusinessRules.Warehouse;
using WHMS.Domain.Entities;
using WHMS.Domain.ValueObjects;

namespace WHMS.Application.Features.Command.Warehouse.CreateWarehouse;

public class CreateWarehouseCommandHandler : IRequestHandler<CreateWarehouseCommandRequest, CreateWarehouseCommandResponse>
{
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly ILocationRepository _locationRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IAuthService _authService;

    public CreateWarehouseCommandHandler(IWarehouseRepository warehouseRepository, IUnitOfWork unitOfWork, ILocationRepository locationRepository, IEmployeeRepository employeeRepository, IAuthService authService)
    {
        _warehouseRepository = warehouseRepository;
        _unitOfWork = unitOfWork;
        _locationRepository = locationRepository;
        _employeeRepository = employeeRepository;
        _authService = authService;
    }

    public async Task<CreateWarehouseCommandResponse> Handle(CreateWarehouseCommandRequest request, CancellationToken cancellationToken)
    {
        Address address = new Address(
            Guid.Parse(request.CityId!),
            Guid.Parse(request.DistrictId!),
            Guid.Parse(request.NeighborhoodId!),
            request.PostalCode!,
            request.AddressLine!
        );

        AddressRules.EnsureIsValid(await _locationRepository.IsAddressValid(address));

        var normalizedName = request.WarehouseName!.ToUpper(CultureInfo.GetCultureInfo("tr-TR"));
        WarehouseRules.EnsureNameIsUnique(await _warehouseRepository.WarehouseNameExists(normalizedName));

        Domain.Entities.Warehouse warehouse = new Domain.Entities.Warehouse 
        { 
            Address = address,
            WarehouseName = request.WarehouseName!,
            NormalizedName = normalizedName
        };

        await _warehouseRepository.CreateWarehouse(warehouse);

        CreateWarehouseCommandResponse response = new() { Warnings = [] };
        if (!string.IsNullOrWhiteSpace(request.ManagerId))
        {
            var user = await _authService.FindByIdAsync(request.ManagerId);
            bool hasManagerRole = user is not null && (await _authService.GetRolesAsync(user)).Contains(ApplicationRole.WarehouseManager);

            var warning = WarehouseRules.ValidateManagerAssignment(user, hasManagerRole, request.ManagerId);
            if (warning is not null)
                response.Warnings.Add(warning);
            else
                user!.WarehouseId = warehouse.Id;
        }

        if (request.StaffIds is not null && request.StaffIds.Count > 0)
            foreach (var staffId in request.StaffIds)
            {
                var user = await _authService.FindByIdAsync(staffId);
                bool hasStaffRole = user is not null && (await _authService.GetRolesAsync(user)).Contains(ApplicationRole.WarehouseStaff);

                var warning = WarehouseRules.ValidateStaffAssignment(user, hasStaffRole, staffId);
                if (warning is not null)
                    response.Warnings.Add(warning);
                else
                    user!.WarehouseId = warehouse.Id;
            }

        await _unitOfWork.CommitAsync();

        response.ResultDto = new()
        {
            WarehouseId = warehouse.Id.ToString(),
            WarehouseName = warehouse.WarehouseName,
            Address = await _locationRepository.ConvertString(address)
        };

        return response;
    }
}