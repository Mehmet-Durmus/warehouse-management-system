using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WHMS.Application.Features.Command.InventoryCount.CreateInventoryCount;
using WHMS.Application.Features.Command.InventoryCount.CreateInventoryCountLine;
using WHMS.Application.Features.Command.InventoryCount.UpdateInventoryCount;
using WHMS.Application.Features.Queries.InventoryCount.GetInventoryCount;
using WHMS.Application.Features.Queries.InventoryCount.GetInventoryCounts;

namespace WHMS.Api.Controllers;


[Route("api/[controller]")]
[ApiController]
public class InventoryCountController : ControllerBase
{
    private readonly IMediator _mediator;

    public InventoryCountController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize(Policy = "LogisticDirector")]
    [HttpGet]
    public async Task<IActionResult> GetInventoryCounts([FromQuery] GetInventoryCountsQueryRequest request)
        => Ok(await _mediator.Send(request));

    [Authorize(Policy = "LogisticDirector")]
    [HttpGet("{inventoryCountId}")]
    public async Task<IActionResult> GetInventoryCount(string inventoryCountId)
        => Ok(await _mediator.Send(new GetInventoryCountQueryRequest {InventoryCountId = inventoryCountId}));

    [Authorize(Policy = "LogisticDirector")]
    [HttpGet("inventory-count-line")]
    public async Task<IActionResult> GetInventoryCountLines()
        => Ok();

    [Authorize(Policy = "LogisticDirector")]
    [HttpGet("inventory-count-line/{inventoryCountLineId}")]
    public async Task<IActionResult> GetInventoryCountLine(string inventoryCountLineId)
        => Ok();

    [Authorize(Policy = "LogisticDirector")]
    [HttpPost]
    public async Task<IActionResult> CreateInventoryCount(CreateInventoryCountCommandRequest request)
        => Ok(await _mediator.Send(request));

    [Authorize(Policy = "LogisticDirector")]
    [HttpPost("inventory-count-line")]
    public async Task<IActionResult> CreateInventoryCountLine(CreateInventoryCountLineCommandRequest request)
        => Ok(await _mediator.Send(request));

    [Authorize(Policy = "LogisticDirector")]
    [HttpPut]
    public async Task<IActionResult> UpdateInventoryCount(UpdateInventoryCountCommandRequest request)
        => Ok(await _mediator.Send(request));

    [Authorize(Policy = "LogisticDirector")]
    [HttpPut("inventory-count-line")]
    public async Task<IActionResult> UpdateInventoryCountLine()
        => Ok();

    [Authorize(Policy = "LogisticDirector")]
    [HttpDelete]
    public async Task<IActionResult> DeleteInventoryCount()
        => Ok();

    [Authorize(Policy = "LogisticDirector")]
    [HttpDelete("inventory-count-line/{inventoryCountLineId}")]
    public async Task<IActionResult> DeleteInventoryCountLine(string inventoryCountLineId)
        => Ok();

}