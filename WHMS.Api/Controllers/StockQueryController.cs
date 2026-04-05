using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WHMS.Api.Common.Models;
using WHMS.Application.Features.Queries.StockQuery.GetTotalProductCount;

namespace WHMS.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class StockQueryController : ControllerBase
{
    private readonly IMediator _mediator;

    public StockQueryController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize(Policy = "LogisticDirector")]
    [HttpGet]
    public async Task<IActionResult> GetTotalProductCount([FromQuery] GetTotalProductCountQueryRequest request)
        => Ok(ApiResponse<GetTotalProductCountQueryResponse>.Success(await _mediator.Send(request)));
}