using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WHMS.Api.Common.Models;
using WHMS.Application.Features.Queries.Location.GetLocationData;

namespace WHMS.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Policy = "LogisticDirector")]
public class LocationController : ControllerBase
{
    private readonly IMediator _mediator;

    public LocationController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetLocationData()
        => Ok(ApiResponse<GetLocationDataQueryResponse>.Success(await _mediator.Send(new GetLocationDataQueryRequest())));
}