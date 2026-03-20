using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WHMS.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class WasteRecordController : ControllerBase
{
    [Authorize(Policy = "LogisticDirector")]
    [HttpPost]
    public async Task<IActionResult> CreateWasteRecord()
        => Ok();

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