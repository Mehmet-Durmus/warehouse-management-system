using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.VisualBasic;
using WHMS.Api.Common.Models;
using WHMS.Application.Features.Command.WasteRecord.CreateWasteRecord;
using WHMS.Application.Features.Command.WasteRecord.DeleteWasteRecord;
using WHMS.Application.Features.Command.WasteRecord.UpdateWasteRecord;
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

    [Authorize(Policy = "ManagerOrStaff")]
    [HttpPost]
    public async Task<IActionResult> CreateWasteRecord(CreateWasteRecordCommandRequest request)
        => Ok(ApiResponse<CreateWasteRecordCommandResponse>.Success(await _mediator.Send(request)));

    [Authorize(Policy = "DirectorManagerOrStaff")]
    [HttpGet]
    public async Task<IActionResult> GetWasteRecords([FromQuery] GetWasteRecordsQueryRequest request)
    {
        var result = await _mediator.Send(request);
        return Ok(ApiResponse<List<GetWasteRecordsResultWasteRecordDto>>
        .SuccessList(result.WasteRecords, new(result.Pagination)));
    }

    [Authorize(Policy = "ManagerOrStaff")]
    [HttpPut]
    public async Task<IActionResult> UpdateWasteRecords(UpdateWasteRecordCommandRequest request)
        => Ok(ApiResponse<UpdateWasteRecordCommandResponse>.Success(await _mediator.Send(request)));
    
    [Authorize(Policy = "LogisticDirector")]
    [HttpDelete("{wasteRecordId}")]
    public async Task<IActionResult> DeleteWasteRecords(string wasteRecordId)
        => Ok(await _mediator.Send(new DeleteWasteRecordCommandRequest { WasteRecordId = wasteRecordId }));
}