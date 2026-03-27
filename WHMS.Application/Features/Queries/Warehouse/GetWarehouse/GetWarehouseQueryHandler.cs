using MediatR;
using WHMS.Application.Abstractions.Persistence;

namespace WHMS.Application.Features.Queries.Warehouse.GetWarehouse;

public class GetWarehouseQueryHandler : IRequestHandler<GetWarehouseQueryRequest, GetWarehouseQueryResponse>
{
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly ILocationRepository _locationRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IStockStateRepository _stockStateRepository;

    public GetWarehouseQueryHandler(IWarehouseRepository warehouseRepository, ILocationRepository locationRepository, IEmployeeRepository employeeRepository, IStockStateRepository stockStateRepository)
    {
        _warehouseRepository = warehouseRepository;
        _locationRepository = locationRepository;
        _employeeRepository = employeeRepository;
        _stockStateRepository = stockStateRepository;
    }

    public async Task<GetWarehouseQueryResponse> Handle(GetWarehouseQueryRequest request, CancellationToken cancellationToken)
    {
        var warehouse = await _warehouseRepository.GetWarehouse(Guid.Parse(request.WarehouseId!))!;

        if (warehouse is null)
            throw new Exception("Warehouse not found.");
        
        var manager = await _employeeRepository.GetManagerByWarehouse(warehouse.Id);
        int productCount = await _stockStateRepository.TotalStockByWarehouse(warehouse.Id);

        return new GetWarehouseQueryResponse
        {
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
            UpdatedAt = warehouse.UpdatedAt,
        };
    }
}