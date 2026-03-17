using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WHMS.Application.Features.Command.Shipment.CreateShipment;
using WHMS.Application.Features.Command.Shipment.CreateShipmentItem;
using WHMS.Application.Features.Queries.Shipment.GetShipment;
using WHMS.Application.Features.Queries.Shipment.GetShipmentItem;
using WHMS.Application.Features.Queries.Shipment.GetShipmentItems;
using WHMS.Application.Features.Queries.Shipment.GetShipments;

namespace WHMS.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ShipmentController : ControllerBase
{
    private readonly IMediator _mediator;

    public ShipmentController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize(Policy = "LogisticDirector")]
    [HttpGet]
    public async Task<IActionResult> GetShipments([FromQuery] GetShipmentsQueryRequest request)
        => Ok(await _mediator.Send(request));

    [Authorize(Policy = "LogisticDirector")]
    [HttpGet("{shipmentId}")]
    public async Task<IActionResult> GetShipment(string shipmentId)
        => Ok(await _mediator.Send(new GetShipmentQueryRequest {ShipmentId = shipmentId}));

    [Authorize(Policy = "LogisticDirector")]
    [HttpGet("shipment-items")]
    public async Task<IActionResult> GetShipmentItems([FromQuery] GetShipmentItemsQueryRequest request)
        => Ok(await _mediator.Send(request));

    [Authorize(Policy = "LogisticDirector")]
    [HttpGet("shipment-items/{shipmentItemId}")]
    public async Task<IActionResult> GetShipmentItem(string shipmentItemId)
        => Ok(await _mediator.Send(new GetShipmentItemQueryRequest {ShipmentItemId = shipmentItemId}));

    [Authorize(Policy = "LogisticDirector")]
    [HttpPost]
    public async Task<IActionResult> CreateShipment(CreateShipmentCommandRequest request)
        => Ok(await _mediator.Send(request));
    
    [Authorize(Policy = "LogisticDirector")]
    [HttpPost("shipment-items")]
    public async Task<IActionResult> CreateShipmentItem(CreateShipmentItemCommandRequest request)
        => Ok(await _mediator.Send(request));

    [Authorize(Policy = "LogisticDirector")]
    [HttpPut]
    public async Task<IActionResult> UpdateShipment()
        => Ok();
    
    [Authorize(Policy = "LogisticDirector")]
    [HttpPut("shipment-items")]
    public async Task<IActionResult> UpdateShipmentItem()
        => Ok();

    [Authorize(Policy = "LogisticDirector")]
    [HttpDelete]
    public async Task<IActionResult> DeleteShipment()
        => Ok();
    
    [Authorize(Policy = "LogisticDirector")]
    [HttpDelete("shipment-items")]
    public async Task<IActionResult> DeleteShipmentItem()
        => Ok();
    
}