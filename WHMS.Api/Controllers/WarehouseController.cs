using System.Net.Http.Headers;
using System.Runtime.InteropServices.Marshalling;
using System.Security.Authentication.ExtendedProtection;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WHMS.Api.Common.Models;
using WHMS.Application.Features.Command.Warehouse.CreateWarehouse;
using WHMS.Application.Features.Command.Warehouse.DeleteWarehouse;
using WHMS.Application.Features.Command.Warehouse.UpdateWarehouse;
using WHMS.Application.Features.Queries.Warehouse.GetAllWarehouses;
using WHMS.Application.Features.Queries.Warehouse.GetWarehouse;

namespace WHMS.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class WarehouseController : ControllerBase
{
    private readonly IMediator _mediator;

    public WarehouseController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize(Policy = "LogisticDirector")]
    [HttpGet]
    public async Task<IActionResult> GetAllWarehouses([FromQuery] GetAllWarehousesQueryRequest request)
        => Ok(await _mediator.Send(request));

    [Authorize(Policy = "LogisticDirector")]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetWarehouse(string id)
        => Ok(await _mediator.Send(new GetWarehouseQueryRequest {WarehouseId = id}));

    [Authorize(Policy = "LogisticDirector")]
    [HttpPost]
    public async Task<IActionResult> CreateWarehouse(CreateWarehouseCommandRequest request)
    {
        var result = await _mediator.Send(request);
        var response = result.Warnings.Count > 0 ?
            ApiResponse<CreateWarehouseResultDto>.SuccessWithWarnings(result.ResultDto, result.Warnings) :
            ApiResponse<CreateWarehouseResultDto>.Success(result.ResultDto);
        
        return CreatedAtAction(nameof(GetWarehouse), new { id = result.ResultDto.WarehouseId }, response);
    }

    [Authorize(Policy = "LogisticDirector")]
    [HttpPut]
    public async Task<IActionResult> UpdateWarehouse(UpdateWarehouseCommandRequest request)
        => Ok(ApiResponse<UpdateWarehouseCommandResponse>.Success(await _mediator.Send(request)));
    

    [Authorize(Policy = "LogisticDirector")]
    [HttpDelete("{warehouseId}")]
    public async Task<IActionResult> DeleteWarehouse(string warehouseId)
    {
        await _mediator.Send(new DeleteWarehouseCommandRequest { WarehouseId = warehouseId });
        return NoContent();
    }
}