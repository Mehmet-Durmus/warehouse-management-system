using System.Drawing;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WHMS.Application.Features.Command.Catalog.CreateCategory;
using WHMS.Application.Features.Command.Catalog.CreateSku;
using WHMS.Application.Features.Command.Catalog.DeleteCategory;
using WHMS.Application.Features.Command.Catalog.DeleteSku;
using WHMS.Application.Features.Command.Catalog.UpdateCategory;
using WHMS.Application.Features.Command.Catalog.UpdateSku;
using WHMS.Application.Features.Queries.Catalog.GetCategories;
using WHMS.Application.Features.Queries.Catalog.GetSku;
using WHMS.Application.Features.Queries.Catalog.GetSkus;

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
        => Ok(await _mediator.Send(new GetCategoriesQueryRequest())); // Eager-loading

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
    public async Task<IActionResult> UpdateCategory(UpdateCategoryCommandRequest request)
        => Ok(await _mediator.Send(request));

    [Authorize(Policy = "LogisticDirector")]
    [HttpDelete("category/{id}")]
    public async Task<IActionResult> DeleteCategory(string id)
        => Ok(await _mediator.Send(new DeleteCategoryCommandRequest {CategoryId = id}));

    [Authorize(Policy = "DirectorOrManager")]
    [HttpGet("skus")]
    public async Task<IActionResult> GetSkus()
        => Ok(await _mediator.Send(new GetSkusQueryRequest()));
    
    [Authorize(Policy = "DirectorOrManager")]
    [HttpGet("skus/{id}")]
    public async Task<IActionResult> GetSku(string id)
        => Ok(await _mediator.Send(new GetSkuQueryRequest {SkuId = id}));

    [Authorize(Policy = "LogisticDirector")]
    [HttpPost("sku")]
    public async Task<IActionResult> CreateSku(CreateSkuCommandRequest request)
        => Ok(await _mediator.Send(request));

    [Authorize(Policy = "LogisticDirector")]
    [HttpPut("sku")]
    public async Task<IActionResult> UpdateSku(UpdateSkuCommandRequest request)
        => Ok(await _mediator.Send(request));

    [Authorize(Policy = "LogisticDirector")]
    [HttpDelete("sku/{id}")]
    public async Task<IActionResult> DeleteSku(string id)
        => Ok(await _mediator.Send(new DeleteSkuCommandRequest {SkuId = id}));
}