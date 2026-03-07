using WHMS.Application.DTOs.Catalog;

namespace WHMS.Application.Features.Queries.Catalog.GetSkus;

public class GetSkusQueryResponse
{
    public List<SkuDto>? Skus { get; set; }
}