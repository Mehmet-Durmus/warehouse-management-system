using Moq;
using WHMS.Application.Abstractions.Infrastructure;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Application.Common.Constants;
using WHMS.Application.Common.Filtering.Filters;
using WHMS.Application.Features.Queries.Delivery.GetDeliveries;

namespace WHMS.Tests.Application.Features.Queries.Delivery.GetDeliveries;

public class GetDeliveriesQueryHandlerTests
{
    private readonly Mock<IDeliveryRepository> _deliveryRepository = new();
    private readonly Mock<ICurrentUserService> _currentUserService = new();
    private readonly GetDeliveriesQueryHandler _handler;
    private readonly Guid _warehouseId = Guid.NewGuid();

    public GetDeliveriesQueryHandlerTests()
    {
        _deliveryRepository.Setup(r => r.GetDeliveries(It.IsAny<DeliveryFilter>(), true)).ReturnsAsync([]);
        _deliveryRepository.Setup(r => r.GetDeliveriesCount(It.IsAny<DeliveryFilter>())).ReturnsAsync(0);
        _handler = new GetDeliveriesQueryHandler(_deliveryRepository.Object, _currentUserService.Object);
    }

    private GetDeliveriesQueryRequest Request() => new()
    {
        WarehouseId = "requested-warehouse", CityId = "requested-city", IsReceived = true, Page = 1, PageSize = 20
    };

    [Fact]
    public async Task Handle_LogisticDirector_UsesRequestedFiltersAsIs()
    {
        _currentUserService.Setup(u => u.Roles).Returns([ApplicationRole.LogisticDirector]);
        DeliveryFilter? captured = null;
        _deliveryRepository
            .Setup(r => r.GetDeliveries(It.IsAny<DeliveryFilter>(), true))
            .Callback<DeliveryFilter, bool>((f, _) => captured = f)
            .ReturnsAsync([]);

        await _handler.Handle(Request(), CancellationToken.None);

        Assert.Equal("requested-warehouse", captured!.WarehouseId);
        Assert.Equal("requested-city", captured.CityId);
        Assert.True(captured.IsReceived);
    }

    [Fact]
    public async Task Handle_WarehouseManager_ForcesOwnWarehouseAndIgnoresCity()
    {
        _currentUserService.Setup(u => u.Roles).Returns([ApplicationRole.WarehouseManager]);
        _currentUserService.Setup(u => u.WarehouseId).Returns(_warehouseId.ToString());
        DeliveryFilter? captured = null;
        _deliveryRepository
            .Setup(r => r.GetDeliveries(It.IsAny<DeliveryFilter>(), true))
            .Callback<DeliveryFilter, bool>((f, _) => captured = f)
            .ReturnsAsync([]);

        await _handler.Handle(Request(), CancellationToken.None);

        Assert.Equal(_warehouseId.ToString(), captured!.WarehouseId);
        Assert.Null(captured.CityId);
        Assert.True(captured.IsReceived);
    }

    [Fact]
    public async Task Handle_WarehouseStaff_ForcesOwnWarehouseAndUnreceivedOnly()
    {
        _currentUserService.Setup(u => u.Roles).Returns([ApplicationRole.WarehouseStaff]);
        _currentUserService.Setup(u => u.WarehouseId).Returns(_warehouseId.ToString());
        DeliveryFilter? captured = null;
        _deliveryRepository
            .Setup(r => r.GetDeliveries(It.IsAny<DeliveryFilter>(), true))
            .Callback<DeliveryFilter, bool>((f, _) => captured = f)
            .ReturnsAsync([]);

        // Request asks for IsReceived=true, but staff can only ever see unreceived
        // deliveries - the handler overrides this regardless of what was requested.
        await _handler.Handle(Request(), CancellationToken.None);

        Assert.Equal(_warehouseId.ToString(), captured!.WarehouseId);
        Assert.False(captured.IsReceived);
    }

    [Fact]
    public async Task Handle_NoRecognizedRole_ThrowsInternalUnauthorized()
    {
        _currentUserService.Setup(u => u.Roles).Returns([]);

        var exception = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(Request(), CancellationToken.None));

        Assert.Equal("Internal - Unauthorized", exception.Message);
    }
}
