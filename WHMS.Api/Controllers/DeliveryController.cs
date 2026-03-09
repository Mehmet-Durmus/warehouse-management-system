using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WHMS.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DeliveryController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetDeliveries()
        => Ok();

    [HttpGet("{deliveryId}")]
    public async Task<IActionResult> GetDelivery(string deliveryId)
        => Ok();

    [HttpGet("delivery-items")]
    public async Task<IActionResult> GetDeliveryItems()
        => Ok();

    [HttpGet("delivery-items/{deliveryItemId}")]
    public async Task<IActionResult> GetDeliveryItem(string deliveryItemId)
        => Ok();
    
    [HttpGet("delivery-items/by-delivery/{deliveryId}")]
    public async Task<IActionResult> GetDeliveryItemsByDelivery(string deliveryId)
        => Ok();

    [HttpPost]
    public async Task<IActionResult> CreateDelivery()
        => Ok();

    [HttpPost("delivery-item")]
    public async Task<IActionResult> CreateDeliveryItem()
        => Ok();

    [HttpPut]
    public async Task<IActionResult> UpdateDelivery()
        => Ok();

    [HttpPut("delivery-item")]
    public async Task<IActionResult> UpdateDeliveryItem()
        => Ok();

    [HttpDelete("{deliveryId}")]
    public async Task<IActionResult> DeleteDelivery(string deliveryId)
        => Ok();

    [HttpDelete("delivery-item/{deliveryItemId}")]
    public async Task<IActionResult> DeleteDeliveryItem(string deliveryItemId)
        => Ok();
}