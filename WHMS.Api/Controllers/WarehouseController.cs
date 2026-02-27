using System.Net.Http.Headers;
using System.Runtime.InteropServices.Marshalling;
using System.Security.Authentication.ExtendedProtection;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WHMS.Application.Features.Command.Warehouse.CreateWarehouse;
using WHMS.Application.Features.Command.Warehouse.UpdateWarehouse;
using WHMS.Application.Features.Queries.Warehouse.GetAllWarehouses;

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
    [HttpPost]
    public async Task<IActionResult> CreateWarehouse(CreateWarehouseCommandRequest request)
        => Ok(await _mediator.Send(request));

    [Authorize(Policy = "LogisticDirector")]
    [HttpPut]
    public async Task<IActionResult> UpdateWarehouse(UpdateWarehouseCommandRequest request)
        => Ok(await _mediator.Send(request));
}