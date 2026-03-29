using WHMS.Application.Common.DTOs;
using WHMS.Application.DTOs.Store;

namespace WHMS.Application.Features.Queries.Store.GetStores;

public class GetStoresQueryResponse
{
    public PaginationDto Pagination { get; set; } = null!;
    public List<GetStoresResultStoreDto>? Stores { get; set; }
}