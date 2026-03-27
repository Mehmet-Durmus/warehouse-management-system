using WHMS.Application.Common.DTOs;
using WHMS.Application.DTOs.Warehouse;

namespace WHMS.Application.Features.Queries.Warehouse.GetAllWarehouses;

public class GetAllWarehousesQueryResponse
{
    public PaginationDto Pagination { get; set; } = null!;
    public List<WarehouseDto> Warehouses { get; set; } = null!;
}