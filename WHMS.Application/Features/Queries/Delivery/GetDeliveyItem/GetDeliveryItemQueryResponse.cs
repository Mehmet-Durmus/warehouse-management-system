using WHMS.Application.DTOs.Catalog;

namespace WHMS.Application.Features.Queries.Delivery.GetDeliveryItem;

public class GetDeliveryItemQueryResponse
{
    public string? DeliveryId { get; set; }
    public string? SkuId { get; set; }
    public string? SKUName { get; set; }
    public int Quantity { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid? CreatedById { get; set; }
    public string? CreatedByName { get; set; }
    public string? CreatedByUserName { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Guid? UpdatedById { get; set; }
    public string? UpdatedByName { get; set; }
    public string? UpdatedByUserName { get; set; }
}