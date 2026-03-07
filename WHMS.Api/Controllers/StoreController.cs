using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WHMS.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class StoreController : ControllerBase
{
    [Authorize(Policy = "LogisticDirector")]
    [HttpGet]
    public async Task<IActionResult> GetStores()
        => Ok();

    [Authorize(Policy = "LogisticDirector")]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetStore(string id)
        => Ok();

    [Authorize(Policy = "LogisticDirector")]
    [HttpPost]
    public async Task<IActionResult> CreateStore()
        => Ok();

    [Authorize(Policy = "LogisticDirector")]
    [HttpPut]
    public async Task<IActionResult> UpdateStore()
        => Ok();

    [Authorize(Policy = "LogisticDirector")]
    [HttpDelete]
    public async Task<IActionResult> DeleteStore()
        => Ok();
}