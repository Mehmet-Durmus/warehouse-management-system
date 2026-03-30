using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WHMS.Application.Features.Command.Delivery.CreateDelivery;
using WHMS.Application.Features.Command.Delivery.CreateDeliveryItem;
using WHMS.Application.Features.Command.Delivery.DeleteDelivery;
using WHMS.Application.Features.Command.Delivery.DeleteDeliveryItem;
using WHMS.Application.Features.Command.Delivery.ReceiveDelivery;
using WHMS.Application.Features.Command.Delivery.UpdateDelivery;
using WHMS.Application.Features.Command.Delivery.UpdateDeliveryItem;
using WHMS.Application.Features.Queries.Delivery.GetDeliveries;
using WHMS.Application.Features.Queries.Delivery.GetDelivery;
using WHMS.Application.Features.Queries.Delivery.GetDeliveryItems;
using WHMS.Application.Features.Queries.Delivery.GetDeliveryItem;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using WHMS.Api.Common.Models;
using WHMS.Api.Common.Models.Delivery;

namespace WHMS.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DeliveryController : ControllerBase
{
    private readonly IMediator _mediator;

    public DeliveryController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize(Policy = "DirectorManagerOrStaff")]
    [HttpGet]
    public async Task<IActionResult> GetDeliveries([FromQuery] GetDeliveriesQueryRequest request)
    {
        var result = await _mediator.Send(request);
        return Ok(ApiResponse<List<GetDeliveriesResultDeliveryDto>>
            .SuccessList(result.Deliveries, new(result.Pagination)));
    }

    [Authorize(Policy = "DirectorManagerOrStaff")]
    [HttpGet("{deliveryId}")]
    public async Task<IActionResult> GetDelivery(string deliveryId)
        => Ok(ApiResponse<GetDeliveryQueryResponse>
            .Success(await _mediator.Send(new GetDeliveryQueryRequet {DeliveryId = deliveryId})));

    [Authorize(Policy = "DirectorManagerOrStaff")]
    [HttpGet("{deliveryId}/delivery-items")]
    public async Task<IActionResult> GetDeliveryItems(string deliveryId, [FromQuery] GetDeliveryItemsRequestDto dto)
    {
        GetDeliveryItemsQueryRequest request = new()
        {
            DeliveryId = deliveryId,
            SkuId = dto.SkuId,
            MaxQuantity = dto.MaxQuantity,
            MinQuantity = dto.MinQuantity,
            Page = dto.Page,
            PageSize = dto.PageSize
        };
        var result = await _mediator.Send(request);
        return Ok(ApiResponse<List<GetDeliveryItemsResultDeliveryItemDto>>.SuccessList(result.DeliveryItems, new(result.Pagination)));
    }

    [Authorize(Policy = "DirectorManagerOrStaff")]
    [HttpGet("delivery-items/{deliveryItemId}")]
    public async Task<IActionResult> GetDeliveryItem(string deliveryItemId)
        => Ok(ApiResponse<GetDeliveryItemQueryResponse>
        .Success(await _mediator.Send(new GetDeliveryItemQueryRequest {DeliveryItemId = deliveryItemId})));

    [Authorize(Policy = "LogisticDirector")]
    [HttpPost]
    public async Task<IActionResult> CreateDelivery(CreateDeliveryCommandRequest request)
        => Ok(ApiResponse<CreateDeliveryCommandResponse>.Success(await _mediator.Send(request)));

    [Authorize(Policy = "LogisticDirector")]
    [HttpPost("delivery-items")]
    public async Task<IActionResult> CreateDeliveryItem(CreateDeliveryItemCommandRequest request)
        => Ok(ApiResponse<CreateDeliveryItemCommandResponse>.Success(await _mediator.Send(request)));

    [Authorize(Policy = "LogisticDirector")]
    [HttpPut]
    public async Task<IActionResult> UpdateDelivery(UpdateDeliveryCommandRequest request)
        => Ok(ApiResponse<UpdateDeliveryCommandResponse>.Success(await _mediator.Send(request)));

    [Authorize(Policy = "LogisticDirector")]
    [HttpPut("delivery-item")]
    public async Task<IActionResult> UpdateDeliveryItem(UpdateDeliveryItemCommandRequest request)
        => Ok(ApiResponse<UpdateDeliveryItemCommandResponse>.Success(await _mediator.Send(request)));

    [Authorize(Policy = "LogisticDirector")]
    [HttpDelete("{deliveryId}")]
    public async Task<IActionResult> DeleteDelivery(string deliveryId)
    {
        await _mediator.Send(new DeleteDeliveryCommandRequest {DeliveryId = deliveryId});
        return NoContent();
    }

    [Authorize(Policy = "LogisticDirector")]
    [HttpDelete("delivery-item/{deliveryItemId}")]
    public async Task<IActionResult> DeleteDeliveryItem(string deliveryItemId)
    {
        await _mediator.Send(new DeleteDeliveryItemCommandRequest {DeliveryItemId = deliveryItemId});
        return NoContent();
    }

    [Authorize(Policy = "ManagerOrStaff")]
    [HttpPost("receive-delivery")]
    public async Task<IActionResult> ReceiveDelivery(ReceiveDeliveryCommandRequest request)
        => Ok(await _mediator.Send(request));
}