using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WHMS.Api.Controllers;


[Route("api/[controller]")]
[ApiController]
public class InventoryCountController : ControllerBase
{
    [Authorize(Policy = "LogisticDirector")]
    [HttpGet]
    public async Task<IActionResult> GetInventoryCounts()
        => Ok();

    [Authorize(Policy = "LogisticDirector")]
    [HttpGet("{inventoryCountId}")]
    public async Task<IActionResult> GetInventoryCount(string inventoryCountId)
        => Ok();

    [Authorize(Policy = "LogisticDirector")]
    [HttpGet("inventory-count-line")]
    public async Task<IActionResult> GetInventoryCountLines()
        => Ok();

    [Authorize(Policy = "LogisticDirector")]
    [HttpGet("inventory-count-line/{inventoryCountLineId}")]
    public async Task<IActionResult> GetInventoryCountLine(string inventoryCountLineId)
        => Ok();

    [Authorize(Policy = "LogisticDirector")]
    [HttpPost]
    public async Task<IActionResult> CreateInventoryCount()
        => Ok();

    [Authorize(Policy = "LogisticDirector")]
    [HttpPost("inventory-count-line")]
    public async Task<IActionResult> CreateInventoryCountLine()
        => Ok();

    [Authorize(Policy = "LogisticDirector")]
    [HttpPut]
    public async Task<IActionResult> UpdateInventoryCount()
        => Ok();

    [Authorize(Policy = "LogisticDirector")]
    [HttpPut("inventory-count-line")]
    public async Task<IActionResult> UpdateInventoryCountLine()
        => Ok();

    [Authorize(Policy = "LogisticDirector")]
    [HttpDelete]
    public async Task<IActionResult> DeleteInventoryCount()
        => Ok();

    [Authorize(Policy = "LogisticDirector")]
    [HttpDelete("inventory-count-line/{inventoryCountLineId}")]
    public async Task<IActionResult> DeleteInventoryCountLine(string inventoryCountLineId)
        => Ok();

}