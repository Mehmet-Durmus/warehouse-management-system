using MediatR;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Application.Common.Filtering.Filters;
using WHMS.Application.DTOs.Warehouse;

namespace WHMS.Application.Features.Queries.Warehouse.GetAllWarehouses;

public class GetAllWarehousesQueryHandler : IRequestHandler<GetAllWarehousesQueryRequest, GetAllWarehousesQueryResponse>
{
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly ILocationRepository _locationRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IStockStateRepository _stockStateRepository;

    public GetAllWarehousesQueryHandler(IWarehouseRepository warehouseRepository, ILocationRepository locationRepository, IEmployeeRepository employeeRepository, IStockStateRepository stockStateRepository)
    {
        _warehouseRepository = warehouseRepository;
        _locationRepository = locationRepository;
        _employeeRepository = employeeRepository;
        _stockStateRepository = stockStateRepository;
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
            SkuIds = request.SkuIds,
            PageSize = request.PageSize,
            CreatedAfter = request.CreatedAfter,
            CreatedBefore = request.CreatedBefore,
            UpdatedAfter = request.UpdatedAfter,
            UpdatedBefore = request.UpdatedBefore
        };

        var warehouses = await _warehouseRepository.GetAllWarehouses(filter);
        int count = await _warehouseRepository.GetWarehousesCount(filter);
        
        var result = new GetAllWarehousesQueryResponse { Warehouses = [] };

        result.Pagination = new(
            request.Page,
            request.PageSize,
            (int)Math.Ceiling((double)count / request.PageSize)
        );

        foreach (var warehouse in warehouses)
        {
            var manager = await _employeeRepository.GetManagerByWarehouse(warehouse.Id);
            int productCount = await _stockStateRepository.TotalStockByWarehouse(warehouse.Id);
            result.Warehouses.Add(new()
            {
                WarehouseId = warehouse.Id.ToString(),
                WarehouseName = warehouse.WarehouseName,
                ManagerId = manager?.Id.ToString(),
                ManagerUsername = manager?.UserName,
                ManagerName = manager?.FullName,
                ProductCount = productCount,
                CityId = warehouse.Address.CityId.ToString(),
                DistrictId = warehouse.Address.DistrictId.ToString(),
                NeighborhoodId = warehouse.Address.NeighborhoodId.ToString(),
                Address = await _locationRepository.ConvertString(warehouse.Address),
                CreatedAt = warehouse.CreatedAt,
                UpdatedAt = warehouse.UpdatedAt
            });
        }
        return result;
    }
}