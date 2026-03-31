using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WHMS.Api.Common.Models;
using WHMS.Application.Features.Command.Shipment.CreateShipment;
using WHMS.Application.Features.Command.Shipment.CreateShipmentItem;
using WHMS.Application.Features.Command.Shipment.DeleteShipment;
using WHMS.Application.Features.Command.Shipment.DeleteShipmentItem;
using WHMS.Application.Features.Command.Shipment.SendShipment;
using WHMS.Application.Features.Command.Shipment.UpdateShipment;
using WHMS.Application.Features.Command.Shipment.UpdateShipmentItem;
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

    [Authorize(Policy = "DirectorManagerOrStaff")]
    [HttpGet]
    public async Task<IActionResult> GetShipments([FromQuery] GetShipmentsQueryRequest request)
    {
        var result = await _mediator.Send(request);
        return Ok(ApiResponse<List<GetShipmentsResultShipmentDto>>.SuccessList(result.Shipments, new(result.Pagination)));
    }

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
        => Ok(ApiResponse<CreateShipmentCommandResponse>.Success(await _mediator.Send(request)));
    
    [Authorize(Policy = "LogisticDirector")]
    [HttpPost("shipment-items")]
    public async Task<IActionResult> CreateShipmentItem(CreateShipmentItemCommandRequest request)
        => Ok(ApiResponse<CreateShipmentItemCommandResponse>.Success(await _mediator.Send(request)));

    [Authorize(Policy = "LogisticDirector")]
    [HttpPut]
    public async Task<IActionResult> UpdateShipment(UpdateShipmentCommandRequest request)
        => Ok(ApiResponse<UpdateShipmentCommandResponse>.Success(await _mediator.Send(request)));
    
    [Authorize(Policy = "LogisticDirector")]
    [HttpPut("shipment-items")]
    public async Task<IActionResult> UpdateShipmentItem(UpdateShipmentItemCommandRequest request)
        => Ok(ApiResponse<UpdateShipmentItemCommandResponse>.Success(await _mediator.Send(request)));

    [Authorize(Policy = "LogisticDirector")]
    [HttpDelete("{shipmentId}")]
    public async Task<IActionResult> DeleteShipment(string shipmentId)
    {
        await _mediator.Send(new DeleteShipmentCommandRequest {ShipmentId = shipmentId});
        return NoContent();
    }
    
    [Authorize(Policy = "LogisticDirector")]
    [HttpDelete("shipment-items/{shipmentItemId}")]
    public async Task<IActionResult> DeleteShipmentItem(string shipmentItemId)
    {
        await _mediator.Send(new DeleteShipmentItemCommandRequest {ShipmentItemId = shipmentItemId});
        return NoContent();
    }

    [Authorize(Policy = "ManagerOrStaff")]
    [HttpPost("send-shipment")]
    public async Task<IActionResult> SendShipment(SendShipmentCommandRequest request)
    {
        var result = await _mediator.Send(request);
        if (result.Errors is null)
            return NoContent();
        return Ok(result);
    }
    
}