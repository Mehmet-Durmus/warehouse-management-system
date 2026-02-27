using MediatR;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Application.DTOs.Warehouse;

namespace WHMS.Application.Features.Queries.Warehouse.GetAllWarehouses;

public class GetAllWarehousesQueryHandler : IRequestHandler<GetAllWarehousesQueryRequest, GetAllWarehousesQueryResponse>
{
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly ILocationRepository _locationRepository;

    public GetAllWarehousesQueryHandler(IWarehouseRepository warehouseRepository, ILocationRepository locationRepository)
    {
        _warehouseRepository = warehouseRepository;
        _locationRepository = locationRepository;
    }

    public async Task<GetAllWarehousesQueryResponse> Handle(GetAllWarehousesQueryRequest request, CancellationToken cancellationToken)
    {
        List<WHMS.Domain.Entities.Warehouse> warehouses = await _warehouseRepository.GetAllWarehouses();
        var result = new GetAllWarehousesQueryResponse {Warehouses = []};
        foreach (var warehouse in warehouses)
            result.Warehouses.Add(new WarehouseDto
            {
                Id = warehouse.Id.ToString(),
                City = await _locationRepository.GetCityName(warehouse.Address.CityId),
                District = await _locationRepository.GetDistrictName(warehouse.Address.DistrictId),
                Neighborhood = await _locationRepository.GetNeighborhoodName(warehouse.Address.NeighborhoodId),
                AddressLine = warehouse.Address.AddressLine,
                CreatedAt = warehouse.CreatedAt,
                UpdatedAt = warehouse.UpdatedAt
            });
        return result;
    }
}