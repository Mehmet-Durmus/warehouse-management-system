using Moq;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Application.Features.Queries.Warehouse.GetWarehouse;
using WHMS.Domain.Entities;
using WHMS.Domain.ValueObjects;

namespace WHMS.Tests.Application.Features.Queries.Warehouse.GetWarehouse;

public class GetWarehouseQueryHandlerTests
{
    private readonly Mock<IWarehouseRepository> _warehouseRepository = new();
    private readonly Mock<ILocationRepository> _locationRepository = new();
    private readonly Mock<IEmployeeRepository> _employeeRepository = new();
    private readonly Mock<IStockStateRepository> _stockStateRepository = new();
    private readonly GetWarehouseQueryHandler _handler;
    private readonly Guid _warehouseId = Guid.NewGuid();

    public GetWarehouseQueryHandlerTests()
    {
        _handler = new GetWarehouseQueryHandler(_warehouseRepository.Object, _locationRepository.Object, _employeeRepository.Object, _stockStateRepository.Object);
    }

    private GetWarehouseQueryRequest Request() => new() { WarehouseId = _warehouseId.ToString() };

    [Fact]
    public async Task Handle_WarehouseNotFound_Throws()
    {
        _warehouseRepository.Setup(r => r.GetWarehouse(_warehouseId)).ReturnsAsync((WHMS.Domain.Entities.Warehouse)null!);

        var exception = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(Request(), CancellationToken.None));

        Assert.Equal("Warehouse not found.", exception.Message);
    }

    [Fact]
    public async Task Handle_WarehouseHasNoManagerYet_ReturnsNullManagerFields()
    {
        var address = new Address(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "34000", "Cadde");
        var warehouse = new WHMS.Domain.Entities.Warehouse { Id = _warehouseId, WarehouseName = "Depo", NormalizedName = "DEPO", Address = address };
        _warehouseRepository.Setup(r => r.GetWarehouse(_warehouseId)).ReturnsAsync(warehouse);
        _employeeRepository.Setup(r => r.GetManagerByWarehouse(_warehouseId)).ReturnsAsync((ApplicationUser)null!);
        _stockStateRepository.Setup(r => r.TotalStockByWarehouse(_warehouseId)).ReturnsAsync(0);
        _locationRepository.Setup(r => r.ConvertString(address)).ReturnsAsync("Cadde, 34000");

        var response = await _handler.Handle(Request(), CancellationToken.None);

        Assert.Null(response.ManagerId);
        Assert.Equal(0, response.ProductCount);
    }

    [Fact]
    public async Task Handle_WarehouseHasManager_ReturnsManagerAndProductCount()
    {
        var address = new Address(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "34000", "Cadde");
        var warehouse = new WHMS.Domain.Entities.Warehouse { Id = _warehouseId, WarehouseName = "Depo", NormalizedName = "DEPO", Address = address };
        var manager = new ApplicationUser { Id = Guid.NewGuid(), UserName = "WHM_0001", FullName = "Test Manager" };
        _warehouseRepository.Setup(r => r.GetWarehouse(_warehouseId)).ReturnsAsync(warehouse);
        _employeeRepository.Setup(r => r.GetManagerByWarehouse(_warehouseId)).ReturnsAsync(manager);
        _stockStateRepository.Setup(r => r.TotalStockByWarehouse(_warehouseId)).ReturnsAsync(42);
        _locationRepository.Setup(r => r.ConvertString(address)).ReturnsAsync("Cadde, 34000");

        var response = await _handler.Handle(Request(), CancellationToken.None);

        Assert.Equal(manager.Id.ToString(), response.ManagerId);
        Assert.Equal("Test Manager", response.ManagerName);
        Assert.Equal(42, response.ProductCount);
    }
}
