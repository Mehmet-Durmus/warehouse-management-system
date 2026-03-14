using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
    public async Task<IActionResult> GetShipments()
        => Ok();

    [Authorize(Policy = "LogisticDirector")]
    [HttpGet("{shipmentId}")]
    public async Task<IActionResult> GetShipment()
        => Ok();

    [Authorize(Policy = "LogisticDirector")]
    [HttpGet("shipment-items")]
    public async Task<IActionResult> GetShipmentItems()
        => Ok();

    [Authorize(Policy = "LogisticDirector")]
    [HttpGet("shipment-items/{shipmentItemId}")]
    public async Task<IActionResult> GetShipmentItem()
        => Ok();

    [Authorize(Policy = "LogisticDirector")]
    [HttpPost]
    public async Task<IActionResult> CreateShipment()
        => Ok();
    
    [Authorize(Policy = "LogisticDirector")]
    [HttpPost("shipment-items")]
    public async Task<IActionResult> CreateShipmentItem()
        => Ok();

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