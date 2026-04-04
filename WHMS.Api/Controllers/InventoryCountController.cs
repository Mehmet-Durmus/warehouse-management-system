using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WHMS.Api.Common.Models;
using WHMS.Api.Common.Models.InventoryCount;
using WHMS.Application.Features.Command.InventoryCount.CompleteInventoryCount;
using WHMS.Application.Features.Command.InventoryCount.CreateInventoryCount;
using WHMS.Application.Features.Command.InventoryCount.CreateInventoryCountLine;
using WHMS.Application.Features.Command.InventoryCount.DeleteInventoryCount;
using WHMS.Application.Features.Command.InventoryCount.DeleteInventoryCountLine;
using WHMS.Application.Features.Command.InventoryCount.UpdateInventoryCountLine;
using WHMS.Application.Features.Queries.InventoryCount.GetInventoryCount;
using WHMS.Application.Features.Queries.InventoryCount.GetInventoryCountLines;
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

    [Authorize(Policy = "DirectorManagerOrStaff")]
    [HttpGet]
    public async Task<IActionResult> GetInventoryCounts([FromQuery] GetInventoryCountsQueryRequest request)
    {
        var result = await _mediator.Send(request);
        return Ok(ApiResponse<List<GetInventoryCountsResultInventoryCountDto>>
        .SuccessList(result.InventoryCounts, new(result.Pagination)));
    }

    [Authorize(Policy = "DirectorManagerOrStaff")]
    [HttpGet("{inventoryCountId}")]
    public async Task<IActionResult> GetInventoryCount(string inventoryCountId)
        => Ok(ApiResponse<GetInventoryCountQueryResponse>
        .Success(await _mediator.Send(new GetInventoryCountQueryRequest {InventoryCountId = inventoryCountId})));

    [Authorize(Policy = "DirectorManagerOrStaff")]
    [HttpGet("{inventoryCountId}/inventory-count-lines")]
    public async Task<IActionResult> GetInventoryCountLines(string inventoryCountId, [FromQuery] GetInventoryCountLinesRequestDto dto)
    {
        GetInventoryCountLinesQueryRequest request = new()
        {
            InventoryCountId = inventoryCountId,
            SkuId = dto.SkuId,
            MaxQuantity = dto.MaxQuantity,
            MinQuantity = dto.MinQuantity,
            MaxVariance = dto.MaxVariance,
            MinVariance = dto.MinVariance,
            Page = dto.Page,
            PageSize = dto.PageSize
        };
        var result = await _mediator.Send(request);
        return Ok(ApiResponse<List<GetInventoryCountLinesResultInventoryCountLineDto>>
        .SuccessList(result.InventoryCountLines, new(result.Pagination)));

    }

    [Authorize(Policy = "WarehouseManager")]
    [HttpPost]
    public async Task<IActionResult> CreateInventoryCount()
        => Ok(ApiResponse<CreateInventoryCountCommandResponse>.Success(await _mediator.Send(new CreateInventoryCountCommandRequest())));

    [Authorize(Policy = "ManagerOrStaff")]
    [HttpPost("complete-inventory-count")]
    public async Task<IActionResult> CompleteInventoryCount(CompleteInventoryCountCommandRequest request)
    {
        await _mediator.Send(request);
        return NoContent();
    }

    [Authorize(Policy = "ManagerOrStaff")]
    [HttpPost("inventory-count-line")]
    public async Task<IActionResult> CreateInventoryCountLine(CreateInventoryCountLineCommandRequest request)
        => Ok(ApiResponse<CreateInventoryCountLineCommandResponse>.Success(await _mediator.Send(request)));

    [Authorize(Policy = "ManagerOrStaff")]
    [HttpPut("inventory-count-line")]
    public async Task<IActionResult> UpdateInventoryCountLine(UpdateInventoryCountLineCommandRequest request)
        => Ok(ApiResponse<UpdateInventoryCountLineCommandResponse>.Success(await _mediator.Send(request)));

    [Authorize(Policy = "LogisticDirector")]
    [HttpDelete("{inventoryCountId}")]
    public async Task<IActionResult> DeleteInventoryCount(string inventoryCountId)
        => Ok(await _mediator.Send(new DeleteInventoryCountCommandRequest { InventoryCountId = inventoryCountId }));

    [Authorize(Policy = "LogisticDirector")]
    [HttpDelete("inventory-count-line/{inventoryCountLineId}")]
    public async Task<IActionResult> DeleteInventoryCountLine(string inventoryCountLineId)
        => Ok(await _mediator.Send(new DeleteInventoryCountLineCommandRequest { InventoryCountLineId = inventoryCountLineId }));

}