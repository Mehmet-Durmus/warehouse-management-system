using MediatR;
using WHMS.Application.Abstractions.Persistence;

namespace WHMS.Application.Features.Queries.Warehouse.GetWarehouse;

public class GetWarehouseQueryHandler : IRequestHandler<GetWarehouseQueryRequest, GetWarehouseQueryResponse>
{
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly ILocationRepository _locationRepository;

    public GetWarehouseQueryHandler(IWarehouseRepository warehouseRepository, ILocationRepository locationRepository)
    {
        _warehouseRepository = warehouseRepository;
        _locationRepository = locationRepository;
    }

    public async Task<GetWarehouseQueryResponse> Handle(GetWarehouseQueryRequest request, CancellationToken cancellationToken)
    {
        var warehouse = await _warehouseRepository.GetWarehouse(Guid.Parse(request.WarehouseId!))!;

        if (warehouse is null)
            throw new Exception("Warehouse not found.");
        
        return new GetWarehouseQueryResponse
        {
            WarehouseName = warehouse.WarehouseName,
            Id = warehouse.Id.ToString(),
            City = await _locationRepository.GetCityName(warehouse.Address.CityId),
            District = await _locationRepository.GetDistrictName(warehouse.Address.DistrictId),
            Neighborhood = await _locationRepository.GetNeighborhoodName(warehouse.Address.NeighborhoodId),
            AddressLine = warehouse.Address.AddressLine,
            CreatedAt = warehouse.CreatedAt,
            UpdatedAt = warehouse.UpdatedAt,
        };
    }
}