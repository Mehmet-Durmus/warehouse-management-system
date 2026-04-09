using System.Net.Http.Headers;
using System.Runtime.InteropServices.Marshalling;
using System.Security.Authentication.ExtendedProtection;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WHMS.Api.Common.Models;
using WHMS.Api.Common.Models.Warehouse;
using WHMS.Application.Features.Command.Warehouse.AssignEmployees;
using WHMS.Application.Features.Command.Warehouse.CreateWarehouse;
using WHMS.Application.Features.Command.Warehouse.DeleteWarehouse;
using WHMS.Application.Features.Command.Warehouse.UpdateWarehouse;
using WHMS.Application.Features.Queries.Warehouse.GetAllWarehouses;
using WHMS.Application.Features.Queries.Warehouse.GetWarehouse;
using WHMS.Application.Features.Queries.Warehouse.GetWarehouseCount;

namespace WHMS.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Policy = "LogisticDirector")]
public class WarehouseController : ControllerBase
{
    private readonly IMediator _mediator;

    public WarehouseController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("counts")]
    public async Task<IActionResult> GetWarehouseCount()
        => Ok(ApiResponse<GetWarehouseCountQueryResponse>
        .Success(await _mediator.Send(new GetWarehouseCountQueryRequest())));

    [HttpGet]
    public async Task<IActionResult> GetAllWarehouses([FromQuery] GetAllWarehousesQueryRequest request)
    {
        var result = await _mediator.Send(request);
        return Ok(ApiResponse<List<GetAllWarehousesResultWarehouseDto>>
            .SuccessList(result.Warehouses, new(result.Pagination)));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetWarehouse(string id)
        => Ok(ApiResponse<GetWarehouseQueryResponse>.
            Success(await _mediator.Send(new GetWarehouseQueryRequest {WarehouseId = id})));

    [HttpPost]
    public async Task<IActionResult> CreateWarehouse(CreateWarehouseCommandRequest request)
    {
        var result = await _mediator.Send(request);
        var response = result.Warnings.Count > 0 ?
            ApiResponse<CreateWarehouseResultDto>.SuccessWithWarnings(result.ResultDto, result.Warnings) :
            ApiResponse<CreateWarehouseResultDto>.Success(result.ResultDto);
        
        return Ok(response);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateWarehouse(UpdateWarehouseCommandRequest request)
        => Ok(ApiResponse<UpdateWarehouseCommandResponse>.Success(await _mediator.Send(request)));
    

    [HttpDelete("{warehouseId}")]
    public async Task<IActionResult> DeleteWarehouse(string warehouseId)
    {
        await _mediator.Send(new DeleteWarehouseCommandRequest { WarehouseId = warehouseId });
        return NoContent();
    }

    [HttpPost("{id}/employees")]
    public async Task<IActionResult> AssignEmployees(string id, AssignEmployeesRequestDto dto)
    {
        var result = await _mediator.Send(new AssignEmployeesCommandRequest
        {
            WarehouseId = id,
            ManagerId = dto.ManagerId,
            StaffIds = dto.StaffIds
        });
        if (result.Warnings is not null && result.Warnings.Count > 0)
            return Ok(ApiResponse.SuccessWithWarnings(result.Warnings));
        
        return NoContent();
    }
}