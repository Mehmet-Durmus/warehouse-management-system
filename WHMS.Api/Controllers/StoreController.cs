using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WHMS.Application.Features.Command.Store.CreateStore;
using WHMS.Application.Features.Command.Store.DeleteStore;
using WHMS.Application.Features.Command.Store.UpdateStore;
using WHMS.Application.Features.Queries.Store.GetStores;
using WHMS.Application.Features.Queries.Store.GetStore;
using WHMS.Api.Common.Models;

namespace WHMS.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class StoreController : ControllerBase
{
    private readonly IMediator _mediator;

    public StoreController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize(Policy = "LogisticDirector")]
    [HttpGet]
    public async Task<IActionResult> GetStores([FromQuery] GetStoresQueryRequest request)
    {
        var result = await _mediator.Send(request);
        return Ok(ApiResponse<List<GetStoresResultStoreDto>>.SuccessList(result.Stores!, new(result.Pagination)));
    }

    [Authorize(Policy = "LogisticDirector")]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetStore(string id)
        => Ok(ApiResponse<GetStoreQueryResponse>.Success(await _mediator.Send(new GetStoreQueryRequest {StoreId = id})));

    [Authorize(Policy = "LogisticDirector")]
    [HttpPost]
    public async Task<IActionResult> CreateStore(CreateStoreCommandRequest request)
        => Ok(ApiResponse<CreateStoreCommandResponse>.Success(await _mediator.Send(request)));

    [Authorize(Policy = "LogisticDirector")]
    [HttpPut]
    public async Task<IActionResult> UpdateStore(UpdateStoreCommandRequest request)
        => Ok(await _mediator.Send(request));

    [Authorize(Policy = "LogisticDirector")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteStore(string id)
        => Ok(await _mediator.Send(new DeleteStoreCommandRequest {StoreId = id}));
}