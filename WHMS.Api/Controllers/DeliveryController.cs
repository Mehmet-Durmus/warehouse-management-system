using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WHMS.Application.Features.Command.Delivery.CreateDelivery;
using WHMS.Application.Features.Command.Delivery.CreateDeliveryItem;
using WHMS.Application.Features.Queries.Delivery.GetDeliveries;

namespace WHMS.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DeliveryController : ControllerBase
{
    private readonly IMediator _mediator;

    public DeliveryController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetDeliveries([FromQuery] GetDeliveriesQueryRequest request)
        => Ok(await _mediator.Send(request));

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
    public async Task<IActionResult> CreateDelivery(CreateDeliveryCommandRequest request)
        => Ok(await _mediator.Send(request));

    [HttpPost("delivery-item")]
    public async Task<IActionResult> CreateDeliveryItem(CreateDeliveryItemCommandRequest request)
        => Ok(await _mediator.Send(request));

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