using WHMS.Application.Common.DTOs;

namespace WHMS.Application.Features.Queries.Warehouse.GetAllWarehouses;

public class GetAllWarehousesQueryResponse
{
    public PaginationDto Pagination { get; set; } = null!;
    public List<GetAllWarehousesResultWarehouseDto> Warehouses { get; set; } = null!;
}