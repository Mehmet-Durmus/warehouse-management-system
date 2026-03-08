using WHMS.Application.DTOs.Store;

namespace WHMS.Application.Features.Queries.Store.GetStores;

public class GetStoresQueryResponse
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPage { get; set; }
    public List<StoreDto>? Stores { get; set; }
}