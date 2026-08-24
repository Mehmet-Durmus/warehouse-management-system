using Moq;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Application.Common.Filtering.Filters;
using WHMS.Application.Features.Command.Store.DeleteStore;
using WHMS.Domain.Entities;

namespace WHMS.Tests.Application.Features.Command.Store.DeleteStore;

public class DeleteStoreCommandHandlerTests
{
    private readonly Mock<IStoreRepository> _storeRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<IShipmentRepository> _shipmentRepository = new();
    private readonly DeleteStoreCommandHandler _handler;
    private readonly Guid _storeId = Guid.NewGuid();

    public DeleteStoreCommandHandlerTests()
    {
        _handler = new DeleteStoreCommandHandler(_storeRepository.Object, _unitOfWork.Object, _shipmentRepository.Object);
    }

    [Fact]
    public async Task Handle_StoreHasUnsentShipments_DeactivatesThemBeforeSoftDeletingStore()
    {
        var pendingShipment = new WHMS.Domain.Entities.Shipment { Id = Guid.NewGuid(), IsActive = true };
        _shipmentRepository
            .Setup(r => r.GetShipments(
                It.Is<ShipmentFilter>(f => f.StoreId == _storeId.ToString() && f.IsSent == false),
                false))
            .ReturnsAsync([pendingShipment]);

        var request = new DeleteStoreCommandRequest { StoreId = _storeId.ToString() };

        await _handler.Handle(request, CancellationToken.None);

        Assert.False(pendingShipment.IsActive);
        _storeRepository.Verify(r => r.SoftDelete(_storeId), Times.Once);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_StoreHasNoUnsentShipments_SoftDeletesStoreAndCommits()
    {
        _shipmentRepository
            .Setup(r => r.GetShipments(It.IsAny<ShipmentFilter>(), false))
            .ReturnsAsync([]);

        var request = new DeleteStoreCommandRequest { StoreId = _storeId.ToString() };

        await _handler.Handle(request, CancellationToken.None);

        _storeRepository.Verify(r => r.SoftDelete(_storeId), Times.Once);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Once);
    }
}
