namespace WHMS.Api.Common.Models.Shipment;

public class GetShipmentItemsRequestDto
{
    public string? SkuId { get; set; }
    public int? MaxQuantity { get; set; }
    public int? MinQuantity { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}