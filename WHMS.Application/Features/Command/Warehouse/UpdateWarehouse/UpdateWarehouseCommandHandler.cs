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
        var warehouse = await _warehouseRepository.GetWarehouse(Guid.Parse(request.WarehouseId))!;
        
        if (warehouse is null)
            throw new Exception("Warehouse not found.");

        bool isAddressValid = await _locationRepository.IsDistrictBelongsToCity(Guid.Parse(request.DistrictId), Guid.Parse(request.CityId))
            && await _locationRepository.IsNeighborhoodBelongsToDistrict(Guid.Parse(request.NeighborhoodId), Guid.Parse(request.DistrictId));
        if (isAddressValid)
            throw new Exception("Address data is invalid.");

        // Mapping
        warehouse.Address = new Address
        {
            CityId = Guid.Parse(request.CityId),
            DistrictId = Guid.Parse(request.DistrictId),
            NeighborhoodId = Guid.Parse(request.NeighborhoodId),
            AddressLine = request.AddressLine,
            PostalCode = request.PostalCode
        };

        _warehouseRepository.UpdateWarehouse(warehouse);
        await _unitOfWork.CommitAsync();
        return new();
    }
}