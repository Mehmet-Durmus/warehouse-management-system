using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WHMS.Application.Features.Command.WasteRecord.CreateWasteRecord;

namespace WHMS.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class WasteRecordController : ControllerBase
{
    private readonly IMediator _mediator;

    public WasteRecordController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize(Policy = "LogisticDirector")]
    [HttpPost]
    public async Task<IActionResult> CreateWasteRecord(CreateWasteRecordCommandRequest request)
        => Ok(await _mediator.Send(request));

    [Authorize(Policy = "LogisticDirector")]
    [HttpGet]
    public async Task<IActionResult> GetWasteRecords()
        => Ok();
    
    [Authorize(Policy = "LogisticDirector")]
    [HttpGet("{wasteRecordId}")]
    public async Task<IActionResult> GetWasteRecord(string wasteRecordId)
        => Ok();

    [Authorize(Policy = "LogisticDirector")]
    [HttpPut]
    public async Task<IActionResult> UpdateWasteRecords()
        => Ok();
    
    [Authorize(Policy = "LogisticDirector")]
    [HttpDelete]
    public async Task<IActionResult> DeleteWasteRecords()
        => Ok();
    
}