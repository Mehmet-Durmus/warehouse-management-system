using System.Globalization;
using MediatR;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Domain.ValueObjects;

namespace WHMS.Application.Features.Command.Warehouse.UpdateWarehouse;

public class UpdateWarehouseCommandHandler : IRequestHandler<UpdateWarehouseCommandRequest, UpdateWarehouseCommandResponse>
{
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly ILocationRepository _locationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateWarehouseCommandHandler(IWarehouseRepository warehouseRepository, IUnitOfWork unitOfWork, ILocationRepository locationRepository)
    {
        _warehouseRepository = warehouseRepository;
        _unitOfWork = unitOfWork;
        _locationRepository = locationRepository;
    }

    public async Task<UpdateWarehouseCommandResponse> Handle(UpdateWarehouseCommandRequest request, CancellationToken cancellationToken)
    {
        var warehouse = await _warehouseRepository.GetWarehouse(Guid.Parse(request.WarehouseId!))!;
        
        if (warehouse is null)
            throw new Exception("Warehouse not found.");

        Address address = new(
            request.CityId!,
            request.DistrictId!,
            request.NeighborhoodId!,request.PostalCode!,
            request.AddressLine!);

        bool isAddressValid = await _locationRepository.IsAddressValid(address);
        if (!isAddressValid)
            throw new Exception("Address data is invalid.");

        var normalizedName = request.WarehouseName!.ToUpper(CultureInfo.GetCultureInfo("tr-TR"));
        bool isNameUsed = await _warehouseRepository.WarehouseNameExists(normalizedName);
        if (!warehouse.NormalizedName.Equals(normalizedName) && isNameUsed)
            throw new Exception("This warehouse name is being used for another warehouse.");
        
        warehouse.Address = address;
        warehouse.NormalizedName = normalizedName;
        warehouse.WarehouseName = request.WarehouseName;

        await _unitOfWork.CommitAsync();
        return new()
        {
            WarehouseId = warehouse.Id.ToString(),
            WarehouseName = warehouse.WarehouseName,
            Address = await _locationRepository.ConvertString(address)
        };
    }
}