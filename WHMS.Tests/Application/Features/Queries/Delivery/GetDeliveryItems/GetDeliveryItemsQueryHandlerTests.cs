using Moq;
using WHMS.Application.Abstractions.Infrastructure;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Application.Common.Constants;
using WHMS.Application.Common.Filtering.Filters;
using WHMS.Application.Features.Queries.Delivery.GetDeliveryItems;
using WHMS.Domain.Entities;

namespace WHMS.Tests.Application.Features.Queries.Delivery.GetDeliveryItems;

public class GetDeliveryItemsQueryHandlerTests
{
    private readonly Mock<IDeliveryRepository> _deliveryRepository = new();
    private readonly Mock<ICurrentUserService> _currentUserService = new();
    private readonly GetDeliveryItemsQueryHandler _handler;
    private readonly Guid _deliveryId = Guid.NewGuid();
    private readonly Guid _warehouseId = Guid.NewGuid();
    private readonly Guid _otherWarehouseId = Guid.NewGuid();

    public GetDeliveryItemsQueryHandlerTests()
    {
        _handler = new GetDeliveryItemsQueryHandler(_deliveryRepository.Object, _currentUserService.Object);
    }

    private void SetCurrentUser(string role) => _currentUserService.Setup(u => u.Roles).Returns([role]);
    private GetDeliveryItemsQueryRequest Request() => new() { DeliveryId = _deliveryId.ToString(), Page = 1, PageSize = 20 };

    [Fact]
    public async Task Handle_DeliveryNotFound_Throws()
    {
        SetCurrentUser(ApplicationRole.LogisticDirector);
        _deliveryRepository.Setup(r => r.GetDelivery(_deliveryId)).ReturnsAsync((WHMS.Domain.Entities.Delivery)null!);

        var exception = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(Request(), CancellationToken.None));

        Assert.Equal("Delivery not found.", exception.Message);
    }

    [Theory]
    [InlineData(ApplicationRole.WarehouseManager, false, false)]
    [InlineData(ApplicationRole.WarehouseStaff, false, false)]
    [InlineData(ApplicationRole.WarehouseStaff, true, true)]
    public async Task Handle_AccessDenied_ThrowsNotFound(string role, bool sameWarehouse, bool received)
    {
        SetCurrentUser(role);
        _currentUserService.Setup(u => u.WarehouseId).Returns(_warehouseId.ToString());
        var delivery = new WHMS.Domain.Entities.Delivery
        {
            Id = _deliveryId,
            WarehouseId = sameWarehouse ? _warehouseId : _otherWarehouseId,
            ReceivedAt = received ? DateTime.Now : null
        };
        _deliveryRepository.Setup(r => r.GetDelivery(_deliveryId)).ReturnsAsync(delivery);

        var exception = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(Request(), CancellationToken.None));

        Assert.Equal("Delivery not found.", exception.Message);
    }

    [Fact]
    public async Task Handle_StaffSameWarehouseNotYetReceived_ReturnsPaginatedItems()
    {
        SetCurrentUser(ApplicationRole.WarehouseStaff);
        _currentUserService.Setup(u => u.WarehouseId).Returns(_warehouseId.ToString());
        var delivery = new WHMS.Domain.Entities.Delivery { Id = _deliveryId, WarehouseId = _warehouseId, ReceivedAt = null };
        _deliveryRepository.Setup(r => r.GetDelivery(_deliveryId)).ReturnsAsync(delivery);
        var item = new DeliveryItem
        {
            Id = Guid.NewGuid(), Quantity = 4,
            Sku = new SKU { Id = Guid.NewGuid(), SKUName = "kola", NormalizedSKUName = "KOLA", Barcode = "8690000000001" }
        };
        _deliveryRepository
            .Setup(r => r.GetDeliveryItems(It.Is<DeliveryItemFilter>(f => f.DeliveryId == _deliveryId.ToString()), true))
            .ReturnsAsync([item]);
        _deliveryRepository.Setup(r => r.GetDeliveryItemsCount(It.IsAny<DeliveryItemFilter>())).ReturnsAsync(1);

        var response = await _handler.Handle(Request(), CancellationToken.None);

        Assert.Single(response.DeliveryItems);
        Assert.Equal("kola", response.DeliveryItems[0].Sku.SkuName);
        Assert.Equal(1, response.Pagination.TotalPage);
    }
}
