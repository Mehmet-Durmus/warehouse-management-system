using System.Globalization;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.IdentityModel.Abstractions;
using WHMS.Application.Abstractions.Persistence;
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

        bool isAddressValid = await _locationRepository.IsAddressValid(address);
        if (!isAddressValid)
            throw new Exception("Address data is invalid.");
        
        var normalizedName = request.WarehouseName!.ToUpper(CultureInfo.GetCultureInfo("tr-TR"));
        bool isNameUsed = await _warehouseRepository.WarehouseNameExists(normalizedName);
        if (isNameUsed)
            throw new Exception("This warehouse name is being used for another warehouse.");

        Domain.Entities.Warehouse warehouse = new Domain.Entities.Warehouse 
        { 
            Address = address,
            WarehouseName = request.WarehouseName!,
            NormalizedName = normalizedName
        };

        CreateWarehouseCommandResponse response = new() { Warnings = [] };
        if (!string.IsNullOrWhiteSpace(request.ManagerId))
        {
            var user = await _authService.FindByIdAsync(request.ManagerId);
            if (user is null)
                response.Warnings.Add($"'{request.ManagerId}' is invalid. Manager not found.");
            else
            {
                var roles = await _authService.GetRolesAsync(user);
                if (!roles.Contains("WarehouseManager"))
                    response.Warnings.Add($"{user.FullName} is not a manager.");
                else
                {
                    if (user.WarehouseId is not null)
                        response.Warnings.Add($"{user.FullName} works at a different warehouse.");
                    else
                        user.WarehouseId = warehouse.Id;
                }
            }
        }

        if (request.StaffIds is not null && request.StaffIds.Count > 0)
            foreach (var staffId in request.StaffIds)
            {
                var user = await _authService.FindByIdAsync(staffId);
                if (user is null)
                    response.Warnings.Add($"'{staffId}' is invalid. Staff member not found.");
                else
                {
                    var roles = await _authService.GetRolesAsync(user);
                    if (!roles.Contains("WarehouseStaff"))
                        response.Warnings.Add($"{user.FullName} is not a staff member.");
                    else
                    {
                        if (user.WarehouseId is not null)
                            response.Warnings.Add($"{user.FullName} works at a different warehouse.");
                        else
                            user.WarehouseId = warehouse.Id;
                    }
                }
                
            }

        await _warehouseRepository.CreateWarehouse(warehouse);
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