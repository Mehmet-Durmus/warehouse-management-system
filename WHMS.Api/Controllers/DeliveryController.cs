using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WHMS.Application.Features.Command.Delivery.CreateDelivery;
using WHMS.Application.Features.Command.Delivery.CreateDeliveryItem;
using WHMS.Application.Features.Command.Delivery.DeleteDelivery;
using WHMS.Application.Features.Command.Delivery.DeleteDeliveryItem;
using WHMS.Application.Features.Command.Delivery.ReceiveDelivery;
using WHMS.Application.Features.Command.Delivery.UpdateDelivery;
using WHMS.Application.Features.Command.Delivery.UpdateDeliveryItem;
using WHMS.Application.Features.Queries.Delivery.GetDeliveries;
using WHMS.Application.Features.Queries.Delivery.GetDelivery;
using WHMS.Application.Features.Queries.Delivery.GetDeliveryItems;
using WHMS.Application.Features.Queries.GetDeliveryItem;

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

    [Authorize(Policy = "LogisticDirector")]
    [HttpGet]
    public async Task<IActionResult> GetDeliveries([FromQuery] GetDeliveriesQueryRequest request)
        => Ok(await _mediator.Send(request));

    [Authorize(Policy = "LogisticDirector")]
    [HttpGet("{deliveryId}")]
    public async Task<IActionResult> GetDelivery(string deliveryId)
        => Ok(await _mediator.Send(new GetDeliveryQueryRequet {DeliveryId = deliveryId}));

    [Authorize(Policy = "LogisticDirector")]
    [HttpGet("delivery-items")]
    public async Task<IActionResult> GetDeliveryItems([FromQuery] GetDeliveryItemsQueryRequest request)
        => Ok(await _mediator.Send(request));

    [Authorize(Policy = "LogisticDirector")]
    [HttpGet("delivery-items/{deliveryItemId}")]
    public async Task<IActionResult> GetDeliveryItem(string deliveryItemId)
        => Ok(await _mediator.Send(new GetDeliveryItemQueryRequest {DeliveryItemId = deliveryItemId}));

    [Authorize(Policy = "LogisticDirector")]
    [HttpPost]
    public async Task<IActionResult> CreateDelivery(CreateDeliveryCommandRequest request)
        => Ok(await _mediator.Send(request));

    [Authorize(Policy = "LogisticDirector")]
    [HttpPost("delivery-items")]
    public async Task<IActionResult> CreateDeliveryItem(CreateDeliveryItemCommandRequest request)
        => Ok(await _mediator.Send(request));

    [Authorize(Policy = "LogisticDirector")]
    [HttpPut]
    public async Task<IActionResult> UpdateDelivery(UpdateDeliveryCommandRequest request)
        => Ok(await _mediator.Send(request));

    [Authorize(Policy = "LogisticDirector")]
    [HttpPut("delivery-item")]
    public async Task<IActionResult> UpdateDeliveryItem(UpdateDeliveryItemCommandRequest request)
        => Ok(await _mediator.Send(request));

    [Authorize(Policy = "LogisticDirector")]
    [HttpDelete("{deliveryId}")]
    public async Task<IActionResult> DeleteDelivery(string deliveryId)
        => Ok(await _mediator.Send(new DeleteDeliveryCommandRequest {DeliveryId = deliveryId}));

    [Authorize(Policy = "LogisticDirector")]
    [HttpDelete("delivery-item/{deliveryItemId}")]
    public async Task<IActionResult> DeleteDeliveryItem(string deliveryItemId)
        => Ok(await _mediator.Send(new DeleteDeliveryItemCommandRequest {DeliveryItemId = deliveryItemId}));

    [Authorize(Policy = "ManagerOrStaff")]
    [HttpPost("receive-delivery")]
    public async Task<IActionResult> ReceiveDelivery(ReceiveDeliveryCommandRequest request)
        => Ok(await _mediator.Send(request));
}