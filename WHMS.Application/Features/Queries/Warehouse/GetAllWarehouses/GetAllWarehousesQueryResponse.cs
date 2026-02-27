using WHMS.Application.DTOs.Warehouse;

namespace WHMS.Application.Features.Queries.Warehouse.GetAllWarehouses;

public class GetAllWarehousesQueryResponse
{
    public List<WarehouseDto>? Warehouses { get; set; }
}