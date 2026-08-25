using Moq;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Application.Common.Filtering.Filters;
using WHMS.Application.Features.Command.Warehouse.DeleteWarehouse;
using WHMS.Domain.Entities;

namespace WHMS.Tests.Application.Features.Command.Warehouse.DeleteWarehouse;

public class DeleteWarehouseCommandHandlerTests
{
    private readonly Mock<IWarehouseRepository> _warehouseRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<IDeliveryRepository> _deliveryRepository = new();
    private readonly Mock<IEmployeeRepository> _employeeRepository = new();
    private readonly DeleteWarehouseCommandHandler _handler;
    private readonly Guid _warehouseId = Guid.NewGuid();

    public DeleteWarehouseCommandHandlerTests()
    {
        _handler = new DeleteWarehouseCommandHandler(
            _warehouseRepository.Object, _unitOfWork.Object, _deliveryRepository.Object, _employeeRepository.Object);
    }

    private DeleteWarehouseCommandRequest Request() => new() { WarehouseId = _warehouseId.ToString() };

    [Fact]
    public async Task Handle_HasPendingDeliveries_ThrowsAndDoesNotUnassignOrDelete()
    {
        _deliveryRepository
            .Setup(r => r.GetDeliveriesCount(It.Is<DeliveryFilter>(f => f.WarehouseId == _warehouseId.ToString() && f.IsReceived == false)))
            .ReturnsAsync(2);

        var exception = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(Request(), CancellationToken.None));

        Assert.Equal("Warehouse cannot be deleted because there are pending deliveries assigned to it.", exception.Message);
        _employeeRepository.Verify(r => r.GetEmployeesByWarehouse(It.IsAny<Guid>()), Times.Never);
        _warehouseRepository.Verify(r => r.SoftDelete(It.IsAny<Guid>()), Times.Never);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_NoPendingDeliveries_UnassignsEmployeesAndSoftDeletesWarehouse()
    {
        var employee1 = new ApplicationUser { Id = Guid.NewGuid(), UserName = "WHM_0001", FullName = "Manager", WarehouseId = _warehouseId };
        var employee2 = new ApplicationUser { Id = Guid.NewGuid(), UserName = "WHS_0001", FullName = "Staff", WarehouseId = _warehouseId };
        _deliveryRepository.Setup(r => r.GetDeliveriesCount(It.IsAny<DeliveryFilter>())).ReturnsAsync(0);
        _employeeRepository.Setup(r => r.GetEmployeesByWarehouse(_warehouseId)).ReturnsAsync([employee1, employee2]);

        await _handler.Handle(Request(), CancellationToken.None);

        Assert.Null(employee1.WarehouseId);
        Assert.Null(employee2.WarehouseId);
        _warehouseRepository.Verify(r => r.SoftDelete(_warehouseId), Times.Once);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Once);
    }
}
