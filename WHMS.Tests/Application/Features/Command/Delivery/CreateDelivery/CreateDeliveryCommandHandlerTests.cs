using Moq;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Application.Features.Command.Delivery.CreateDelivery;
using WHMS.Domain.Exceptions;

namespace WHMS.Tests.Application.Features.Command.Delivery.CreateDelivery;

public class CreateDeliveryCommandHandlerTests
{
    private readonly Mock<IDeliveryRepository> _deliveryRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<IWarehouseRepository> _warehouseRepository = new();
    private readonly CreateDeliveryCommandHandler _handler;
    private readonly Guid _warehouseId = Guid.NewGuid();

    public CreateDeliveryCommandHandlerTests()
    {
        _handler = new CreateDeliveryCommandHandler(_deliveryRepository.Object, _unitOfWork.Object, _warehouseRepository.Object);
    }

    [Fact]
    public async Task Handle_WarehouseNotFound_ThrowsAndDoesNotCreate()
    {
        _warehouseRepository.Setup(r => r.GetWarehouse(_warehouseId)).ReturnsAsync((WHMS.Domain.Entities.Warehouse)null!);

        var request = new CreateDeliveryCommandRequest { WarehouseId = _warehouseId.ToString(), ExpectedArrivalDate = new DateTime(2026, 1, 1) };

        var exception = await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(request, CancellationToken.None));

        Assert.Equal("Warehouse not found.", exception.Message);
        _deliveryRepository.Verify(r => r.CreateDelivery(It.IsAny<WHMS.Domain.Entities.Delivery>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WarehouseFound_CreatesDeliveryAndReturnsResponse()
    {
        var warehouse = new WHMS.Domain.Entities.Warehouse
        {
            Id = _warehouseId,
            WarehouseName = "Ana Depo",
            NormalizedName = "ANA DEPO",
            Address = new WHMS.Domain.ValueObjects.Address(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "34000", "Cadde")
        };
        _warehouseRepository.Setup(r => r.GetWarehouse(_warehouseId)).ReturnsAsync(warehouse);
        var expectedDate = new DateTime(2026, 1, 1);

        var request = new CreateDeliveryCommandRequest { WarehouseId = _warehouseId.ToString(), ExpectedArrivalDate = expectedDate };

        var response = await _handler.Handle(request, CancellationToken.None);

        _deliveryRepository.Verify(r => r.CreateDelivery(It.Is<WHMS.Domain.Entities.Delivery>(d =>
            d.WarehouseId == _warehouseId && d.ExpectedArrivalDate == expectedDate)), Times.Once);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Once);
        Assert.Equal("Ana Depo", response.WarehouseName);
        Assert.Equal(expectedDate, response.ExpectedArrivalDate);
    }
}
