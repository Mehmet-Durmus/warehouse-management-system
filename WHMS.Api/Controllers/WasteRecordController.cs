using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WHMS.Application.Features.Command.WasteRecord.CreateWasteRecord;
using WHMS.Application.Features.Command.WasteRecord.DeleteWasteRecord;
using WHMS.Application.Features.Command.WasteRecord.UpdateWasteRecord;
using WHMS.Application.Features.Queries.WasteRecord.GetWasteRecord;
using WHMS.Application.Features.Queries.WasteRecord.GetWasteRecords;

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
    public async Task<IActionResult> GetWasteRecords([FromQuery] GetWasteRecordsQueryRequest request)
        => Ok(await _mediator.Send(request));
    
    [Authorize(Policy = "LogisticDirector")]
    [HttpGet("{wasteRecordId}")]
    public async Task<IActionResult> GetWasteRecord(string wasteRecordId)
        => Ok(await _mediator.Send(new GetWasteRecordQueryRequest { WasteRecordId = wasteRecordId }));

    [Authorize(Policy = "LogisticDirector")]
    [HttpPut]
    public async Task<IActionResult> UpdateWasteRecords(UpdateWasteRecordCommandRequest request)
        => Ok(await _mediator.Send(request));
    
    [Authorize(Policy = "LogisticDirector")]
    [HttpDelete("{wasteRecordId}")]
    public async Task<IActionResult> DeleteWasteRecords(string wasteRecordId)
        => Ok(await _mediator.Send(new DeleteWasteRecordCommandRequest { WasteRecordId = wasteRecordId }));
}