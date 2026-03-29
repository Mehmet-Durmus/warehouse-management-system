using System.Drawing;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WHMS.Api.Common.Models;
using WHMS.Application.Features.Command.Catalog.CreateCategory;
using WHMS.Application.Features.Command.Catalog.CreateSku;
using WHMS.Application.Features.Command.Catalog.DeleteCategory;
using WHMS.Application.Features.Command.Catalog.DeleteSku;
using WHMS.Application.Features.Command.Catalog.UpdateCategory;
using WHMS.Application.Features.Command.Catalog.UpdateSku;
using WHMS.Application.Features.Queries.Catalog.GetCatalogData;

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

    [Authorize(Policy = "DirectorManagerOrStaff")]
    [HttpGet]
    public async Task<IActionResult> GetCatalogData()
        => Ok(ApiResponse<GetCatalogDataQueryResponse>.Success(await _mediator.Send(new GetCatalogDataQueryRequest())));

    [Authorize(Policy = "LogisticDirector")]
    [HttpPost("category")]
    public async Task<IActionResult> CreateCategory(CreateCategoryCommandRequest request)
        => Ok(ApiResponse<CreateCategoryCommandResponse>.Success(await _mediator.Send(request)));

    [Authorize(Policy = "LogisticDirector")]
    [HttpPut("category")]
    public async Task<IActionResult> UpdateCategory(UpdateCategoryCommandRequest request)
        => Ok(ApiResponse<UpdateCategoryCommandResponse>.Success(await _mediator.Send(request)));

    [Authorize(Policy = "LogisticDirector")]
    [HttpDelete("category/{id}")]
    public async Task<IActionResult> DeleteCategory(string id)
    {
        await _mediator.Send(new DeleteCategoryCommandRequest {CategoryId = id});
        return NoContent();
    }

    [Authorize(Policy = "LogisticDirector")]
    [HttpPost("sku")]
    public async Task<IActionResult> CreateSku(CreateSkuCommandRequest request)
        => Ok(ApiResponse<CreateSkuCommandResponse>.Success(await _mediator.Send(request)));

    [Authorize(Policy = "LogisticDirector")]
    [HttpPut("sku")]
    public async Task<IActionResult> UpdateSku(UpdateSkuCommandRequest request)
        => Ok(ApiResponse<UpdateSkuCommandResponse>.Success(await _mediator.Send(request)));

    [Authorize(Policy = "LogisticDirector")]
    [HttpDelete("sku/{id}")]
    public async Task<IActionResult> DeleteSku(string id)
    {
        await _mediator.Send(new DeleteSkuCommandRequest {SkuId = id});
        return NoContent();
    }
}