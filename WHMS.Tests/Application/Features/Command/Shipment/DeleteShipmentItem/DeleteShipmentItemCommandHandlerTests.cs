using Moq;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Application.Features.Command.Shipment.DeleteShipmentItem;
using WHMS.Domain.Entities;
using WHMS.Domain.Exceptions;

namespace WHMS.Tests.Application.Features.Command.Shipment.DeleteShipmentItem;

public class DeleteShipmentItemCommandHandlerTests
{
    private readonly Mock<IShipmentRepository> _shipmentRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly DeleteShipmentItemCommandHandler _handler;
    private readonly Guid _shipmentItemId = Guid.NewGuid();
    private readonly Guid _shipmentId = Guid.NewGuid();

    public DeleteShipmentItemCommandHandlerTests()
    {
        _handler = new DeleteShipmentItemCommandHandler(_shipmentRepository.Object, _unitOfWork.Object);
    }

    private DeleteShipmentItemCommandRequest Request() => new() { ShipmentItemId = _shipmentItemId.ToString() };

    [Fact]
    public async Task Handle_ShipmentItemNotFound_ThrowsAndDoesNotCommit()
    {
        _shipmentRepository.Setup(r => r.GetShipmentItem(_shipmentItemId)).ReturnsAsync((ShipmentItem)null!);

        var exception = await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(Request(), CancellationToken.None));

        Assert.Equal("Shipment item not found.", exception.Message);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_ParentShipmentNotFound_Throws()
    {
        var item = new ShipmentItem { Id = _shipmentItemId, ShipmentId = _shipmentId, SkuId = Guid.NewGuid(), Quantity = 1 };
        _shipmentRepository.Setup(r => r.GetShipmentItem(_shipmentItemId)).ReturnsAsync(item);
        _shipmentRepository.Setup(r => r.GetShipment(_shipmentId)).ReturnsAsync((WHMS.Domain.Entities.Shipment)null!);

        var exception = await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(Request(), CancellationToken.None));

        Assert.Equal("Shipment not found.", exception.Message);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_ParentShipmentAlreadySent_ThrowsAndDoesNotDelete()
    {
        var item = new ShipmentItem { Id = _shipmentItemId, ShipmentId = _shipmentId, SkuId = Guid.NewGuid(), Quantity = 1 };
        var shipment = new WHMS.Domain.Entities.Shipment { Id = _shipmentId, SendingDate = DateTime.Now };
        _shipmentRepository.Setup(r => r.GetShipmentItem(_shipmentItemId)).ReturnsAsync(item);
        _shipmentRepository.Setup(r => r.GetShipment(_shipmentId)).ReturnsAsync(shipment);

        var exception = await Assert.ThrowsAsync<BusinessRuleViolationException>(() => _handler.Handle(Request(), CancellationToken.None));

        Assert.Equal("Shipment has already been sent.", exception.Message);
        _shipmentRepository.Verify(r => r.DeleteShipmentItem(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShipmentNotYetSent_DeletesItemAndCommits()
    {
        var item = new ShipmentItem { Id = _shipmentItemId, ShipmentId = _shipmentId, SkuId = Guid.NewGuid(), Quantity = 1 };
        var shipment = new WHMS.Domain.Entities.Shipment { Id = _shipmentId, SendingDate = null };
        _shipmentRepository.Setup(r => r.GetShipmentItem(_shipmentItemId)).ReturnsAsync(item);
        _shipmentRepository.Setup(r => r.GetShipment(_shipmentId)).ReturnsAsync(shipment);

        await _handler.Handle(Request(), CancellationToken.None);

        _shipmentRepository.Verify(r => r.DeleteShipmentItem(_shipmentItemId), Times.Once);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Once);
    }
}
