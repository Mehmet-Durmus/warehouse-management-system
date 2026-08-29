using Moq;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Application.Features.Command.Shipment.DeleteShipment;
using WHMS.Domain.Entities;
using WHMS.Domain.Exceptions;

namespace WHMS.Tests.Application.Features.Command.Shipment.DeleteShipment;

public class DeleteShipmentCommandHandlerTests
{
    private readonly Mock<IShipmentRepository> _shipmentRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly DeleteShipmentCommandHandler _handler;
    private readonly Guid _shipmentId = Guid.NewGuid();

    public DeleteShipmentCommandHandlerTests()
    {
        _handler = new DeleteShipmentCommandHandler(_shipmentRepository.Object, _unitOfWork.Object);
    }

    private DeleteShipmentCommandRequest Request() => new() { ShipmentId = _shipmentId.ToString() };

    [Fact]
    public async Task Handle_ShipmentNotFound_ThrowsAndDoesNotCommit()
    {
        _shipmentRepository.Setup(r => r.GetShipment(_shipmentId)).ReturnsAsync((WHMS.Domain.Entities.Shipment)null!);

        var exception = await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(Request(), CancellationToken.None));

        Assert.Equal("Shipment not found.", exception.Message);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_ShipmentAlreadySent_ThrowsAndDoesNotDelete()
    {
        var shipment = new WHMS.Domain.Entities.Shipment { Id = _shipmentId, SendingDate = DateTime.Now };
        _shipmentRepository.Setup(r => r.GetShipment(_shipmentId)).ReturnsAsync(shipment);

        var exception = await Assert.ThrowsAsync<BusinessRuleViolationException>(() => _handler.Handle(Request(), CancellationToken.None));

        Assert.Equal("Shipment has already been sent.", exception.Message);
        _shipmentRepository.Verify(r => r.DeleteShipment(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task Handle_NotSent_DeletesAllItemsThenTheShipmentItself()
    {
        var item1 = new ShipmentItem { Id = Guid.NewGuid(), ShipmentId = _shipmentId, SkuId = Guid.NewGuid(), Quantity = 1 };
        var item2 = new ShipmentItem { Id = Guid.NewGuid(), ShipmentId = _shipmentId, SkuId = Guid.NewGuid(), Quantity = 2 };
        var shipment = new WHMS.Domain.Entities.Shipment { Id = _shipmentId, SendingDate = null, ShipmentItems = [item1, item2] };
        _shipmentRepository.Setup(r => r.GetShipment(_shipmentId)).ReturnsAsync(shipment);

        await _handler.Handle(Request(), CancellationToken.None);

        _shipmentRepository.Verify(r => r.DeleteShipmentItem(item1.Id), Times.Once);
        _shipmentRepository.Verify(r => r.DeleteShipmentItem(item2.Id), Times.Once);
        _shipmentRepository.Verify(r => r.DeleteShipment(_shipmentId), Times.Once);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_NoItems_DeletesShipmentWithoutIteratingItems()
    {
        var shipment = new WHMS.Domain.Entities.Shipment { Id = _shipmentId, SendingDate = null, ShipmentItems = null };
        _shipmentRepository.Setup(r => r.GetShipment(_shipmentId)).ReturnsAsync(shipment);

        await _handler.Handle(Request(), CancellationToken.None);

        _shipmentRepository.Verify(r => r.DeleteShipmentItem(It.IsAny<Guid>()), Times.Never);
        _shipmentRepository.Verify(r => r.DeleteShipment(_shipmentId), Times.Once);
    }
}
