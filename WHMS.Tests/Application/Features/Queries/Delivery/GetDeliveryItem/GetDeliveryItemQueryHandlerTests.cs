using Moq;
using WHMS.Application.Abstractions.Infrastructure;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Application.Common.Constants;
using WHMS.Application.Features.Queries.Delivery.GetDeliveryItem;
using WHMS.Domain.Entities;

namespace WHMS.Tests.Application.Features.Queries.Delivery.GetDeliveryItem;

public class GetDeliveryItemQueryHandlerTests
{
    private readonly Mock<IDeliveryRepository> _deliveryRepository = new();
    private readonly Mock<ICurrentUserService> _currentUserService = new();
    private readonly GetDeliveryItemQueryHandler _handler;
    private readonly Guid _deliveryItemId = Guid.NewGuid();
    private readonly Guid _warehouseId = Guid.NewGuid();
    private readonly Guid _otherWarehouseId = Guid.NewGuid();

    public GetDeliveryItemQueryHandlerTests()
    {
        _handler = new GetDeliveryItemQueryHandler(_deliveryRepository.Object, _currentUserService.Object);
    }

    private void SetCurrentUser(string role) => _currentUserService.Setup(u => u.Roles).Returns([role]);
    private GetDeliveryItemQueryRequest Request() => new() { DeliveryItemId = _deliveryItemId.ToString() };

    private DeliveryItem MakeItem(Guid warehouseId, bool received) => new()
    {
        Id = _deliveryItemId,
        SkuId = Guid.NewGuid(),
        Quantity = 3,
        Sku = new SKU { Id = Guid.NewGuid(), SKUName = "kola", NormalizedSKUName = "KOLA", Barcode = "8690000000001" },
        Delivery = new WHMS.Domain.Entities.Delivery { WarehouseId = warehouseId, ReceivedAt = received ? DateTime.Now : null }
    };

    [Fact]
    public async Task Handle_ItemNotFound_Throws()
    {
        SetCurrentUser(ApplicationRole.LogisticDirector);
        _deliveryRepository.Setup(r => r.GetDeliveryItem(_deliveryItemId)).ReturnsAsync((DeliveryItem)null!);

        var exception = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(Request(), CancellationToken.None));

        Assert.Equal("Delivery item not found", exception.Message);
    }

    [Theory]
    [InlineData(ApplicationRole.WarehouseManager, false, false)]
    [InlineData(ApplicationRole.WarehouseStaff, false, false)]
    [InlineData(ApplicationRole.WarehouseStaff, true, true)]
    public async Task Handle_AccessDenied_ThrowsNotFound(string role, bool sameWarehouse, bool received)
    {
        SetCurrentUser(role);
        _currentUserService.Setup(u => u.WarehouseId).Returns(_warehouseId.ToString());
        var item = MakeItem(sameWarehouse ? _warehouseId : _otherWarehouseId, received);
        _deliveryRepository.Setup(r => r.GetDeliveryItem(_deliveryItemId)).ReturnsAsync(item);

        var exception = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(Request(), CancellationToken.None));

        Assert.Equal("Delivery item not found", exception.Message);
    }

    [Fact]
    public async Task Handle_StaffSameWarehouseNotYetReceived_ReturnsResponse()
    {
        SetCurrentUser(ApplicationRole.WarehouseStaff);
        _currentUserService.Setup(u => u.WarehouseId).Returns(_warehouseId.ToString());
        var item = MakeItem(_warehouseId, received: false);
        _deliveryRepository.Setup(r => r.GetDeliveryItem(_deliveryItemId)).ReturnsAsync(item);

        var response = await _handler.Handle(Request(), CancellationToken.None);

        Assert.Equal("kola", response.Sku.SkuName);
        Assert.Equal(3, response.Quantity);
    }

    [Fact]
    public async Task Handle_LogisticDirector_BypassesWarehouseScoping()
    {
        SetCurrentUser(ApplicationRole.LogisticDirector);
        var item = MakeItem(_otherWarehouseId, received: true);
        _deliveryRepository.Setup(r => r.GetDeliveryItem(_deliveryItemId)).ReturnsAsync(item);

        var response = await _handler.Handle(Request(), CancellationToken.None);

        Assert.Equal("kola", response.Sku.SkuName);
    }
}
