using MediatR;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Application.Common.Filtering.Filters;
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
        WarehouseFilter filter = new WarehouseFilter
        {
            Name = request.Name,
            CityId = request.CityId,
            DistrictId = request.DistrictId,
            NeighborhoodId = request.DistrictId,
            Page = request.Page,
            PageSize = request.PageSize,
            CreatedAfter = request.CreatedAfter,
            CreatedBefore = request.CreatedBefore,
            UpdatedAfter = request.UpdatedAfter,
            UpdatedBefore = request.UpdatedBefore
        };

        var warehouses = await _warehouseRepository.GetAllWarehouses(filter);
        int count = await _warehouseRepository.GetWarehousesCount(filter);
        
        var result = new GetAllWarehousesQueryResponse 
        {
            Page = request.Page,
            PageSize = request.PageSize,
            TotalPage = (int)Math.Ceiling((double)count / request.PageSize),
            Warehouses = []
        };

        foreach (var warehouse in warehouses)
            result.Warehouses.Add(new WarehouseDto
            {
                WarehouseName = warehouse.WarehouseName,
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