using WHMS.Application.Common.DTOs;

namespace WHMS.Application.Features.Queries.Store.GetStores;

public class GetStoresQueryResponse
{
    public PaginationDto Pagination { get; set; } = null!;
    public List<GetStoresResultStoreDto>? Stores { get; set; }
}