using System.Drawing;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WHMS.Application.Features.Command.Catalog;

namespace WHMS.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CatalogController : ControllerBase
{
    private readonly IMediator _mediator;

    public CatalogController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize(Policy = "DirectorOrManager")]
    [HttpGet("categories")]
    public async Task<IActionResult> GetCategories()
        => Ok(); // Eager-loading

    [Authorize(Policy = "DirectorOrManager")]
    [HttpGet("categories7{id}")]
    public async Task<IActionResult> GetCategory(string id)
        => Ok(); // Eager-loading

    [Authorize(Policy = "LogisticDirector")]
    [HttpPost("category")]
    public async Task<IActionResult> CreateCategory(CreateCategoryCommandRequest request)
        => Ok(await _mediator.Send(request));

    [Authorize(Policy = "LogisticDirector")]
    [HttpPut("category")]
    public async Task<IActionResult> UpdateCategory()
        => Ok();

    [Authorize(Policy = "LogisticDirector")]
    [HttpDelete("category/{id}")]
    public async Task<IActionResult> DeleteCategory(string id)
        => Ok();

    [Authorize(Policy = "DirectorOrManager")]
    [HttpGet("skus")]
    public async Task<IActionResult> GetSkus()
        => Ok();
    
    [Authorize(Policy = "DirectorOrManager")]
    [HttpGet("skus/{id}")]
    public async Task<IActionResult> GetSku(string id)
        => Ok();

    [Authorize(Policy = "LogisticDirector")]
    [HttpPost("sku")]
    public async Task<IActionResult> CreateSku()
        => Ok();

    [Authorize(Policy = "LogisticDirector")]
    [HttpPut("sku")]
    public async Task<IActionResult> UpdateSku()
        => Ok();

    [Authorize(Policy = "LogisticDirector")]
    [HttpDelete("sku/{id}")]
    public async Task<IActionResult> DeleteSku(string id)
        => Ok();
}