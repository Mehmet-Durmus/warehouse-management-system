using System.Globalization;
using MediatR;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Domain.ValueObjects;

namespace WHMS.Application.Features.Command.Warehouse.CreateWarehouse;

public class CreateWarehouseCommandHandler : IRequestHandler<CreateWarehouseCommandRequest, CreateWarehouseCommandResponse>
{
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly ILocationRepository _locationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateWarehouseCommandHandler(IWarehouseRepository warehouseRepository, IUnitOfWork unitOfWork, ILocationRepository locationRepository)
    {
        _warehouseRepository = warehouseRepository;
        _unitOfWork = unitOfWork;
        _locationRepository = locationRepository;
    }

    public async Task<CreateWarehouseCommandResponse> Handle(CreateWarehouseCommandRequest request, CancellationToken cancellationToken)
    {
        bool isAddressValid = await _locationRepository.IsAddressValid(
            Guid.Parse(request.CityId!),
            Guid.Parse(request.DistrictId!),
            Guid.Parse(request.NeighborhoodId!));
        if (!isAddressValid)
            throw new Exception("Address data is invalid.");
        
        Address address = new Address
        {
            CityId = Guid.Parse(request.CityId!),
            DistrictId = Guid.Parse(request.DistrictId!),
            NeighborhoodId = Guid.Parse(request.NeighborhoodId!),
            AddressLine = request.AddressLine!,
            PostalCode = request.PostalCode!
        };

        var normalizedName = request.WarehouseName!.ToUpper(CultureInfo.GetCultureInfo("tr-TR"));

        Domain.Entities.Warehouse warehouse = new Domain.Entities.Warehouse 
        { 
            Address = address,
            WarehouseName = request.WarehouseName!,
            NormalizedName = normalizedName
        };
        await _warehouseRepository.CreateWarehouse(warehouse);
        await _unitOfWork.CommitAsync();
        return new();
    }
}