using Moq;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Application.Features.Command.Warehouse.UpdateWarehouse;
using WHMS.Domain.ValueObjects;

namespace WHMS.Tests.Application.Features.Command.Warehouse.UpdateWarehouse;

public class UpdateWarehouseCommandHandlerTests
{
    private readonly Mock<IWarehouseRepository> _warehouseRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<ILocationRepository> _locationRepository = new();
    private readonly UpdateWarehouseCommandHandler _handler;

    private readonly Guid _warehouseId = Guid.NewGuid();
    private readonly Guid _cityId = Guid.NewGuid();
    private readonly Guid _districtId = Guid.NewGuid();
    private readonly Guid _neighborhoodId = Guid.NewGuid();

    public UpdateWarehouseCommandHandlerTests()
    {
        _handler = new UpdateWarehouseCommandHandler(_warehouseRepository.Object, _unitOfWork.Object, _locationRepository.Object);
    }

    private WHMS.Domain.Entities.Warehouse ExistingWarehouse() => new()
    {
        Id = _warehouseId,
        WarehouseName = "market",
        NormalizedName = "MARKET",
        Address = new Address(_cityId, _districtId, _neighborhoodId, "34000", "Eski Cadde No: 1")
    };

    private UpdateWarehouseCommandRequest RequestWithName(string name) => new()
    {
        WarehouseId = _warehouseId.ToString(),
        WarehouseName = name,
        CityId = _cityId.ToString(),
        DistrictId = _districtId.ToString(),
        NeighborhoodId = _neighborhoodId.ToString(),
        AddressLine = "Yeni Cadde No: 2",
        PostalCode = "34100"
    };

    [Fact]
    public async Task Handle_WarehouseNotFound_ThrowsAndDoesNotCommit()
    {
        _warehouseRepository.Setup(r => r.GetWarehouse(_warehouseId)).ReturnsAsync((WHMS.Domain.Entities.Warehouse)null!);

        var exception = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(RequestWithName("market"), CancellationToken.None));

        Assert.Equal("Warehouse not found.", exception.Message);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_AddressInvalid_ThrowsWithoutCheckingNameOrCommitting()
    {
        _warehouseRepository.Setup(r => r.GetWarehouse(_warehouseId)).ReturnsAsync(ExistingWarehouse());
        _locationRepository.Setup(r => r.IsAddressValid(It.IsAny<Address>())).ReturnsAsync(false);

        var exception = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(RequestWithName("market"), CancellationToken.None));

        Assert.Equal("Address data is invalid.", exception.Message);
        _warehouseRepository.Verify(r => r.WarehouseNameExists(It.IsAny<string>()), Times.Never);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_NameChangedAndUnique_UpdatesWarehouseAndCommits()
    {
        var warehouse = ExistingWarehouse();
        _warehouseRepository.Setup(r => r.GetWarehouse(_warehouseId)).ReturnsAsync(warehouse);
        _locationRepository.Setup(r => r.IsAddressValid(It.IsAny<Address>())).ReturnsAsync(true);
        _warehouseRepository.Setup(r => r.WarehouseNameExists("MAGAZA")).ReturnsAsync(false);
        _locationRepository.Setup(r => r.ConvertString(It.IsAny<Address>())).ReturnsAsync("Yeni Cadde No: 2, 34100");

        var response = await _handler.Handle(RequestWithName("magaza"), CancellationToken.None);

        Assert.Equal("magaza", warehouse.WarehouseName);
        Assert.Equal("MAGAZA", warehouse.NormalizedName);
        Assert.Equal(_neighborhoodId, warehouse.Address.NeighborhoodId);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Once);
        Assert.Equal("magaza", response.WarehouseName);
        Assert.Equal("Yeni Cadde No: 2, 34100", response.Address);
    }

    [Fact]
    public async Task Handle_NameChangedButAlreadyUsed_ThrowsAndLeavesWarehouseUnchanged()
    {
        var warehouse = ExistingWarehouse();
        _warehouseRepository.Setup(r => r.GetWarehouse(_warehouseId)).ReturnsAsync(warehouse);
        _locationRepository.Setup(r => r.IsAddressValid(It.IsAny<Address>())).ReturnsAsync(true);
        _warehouseRepository.Setup(r => r.WarehouseNameExists("MAGAZA")).ReturnsAsync(true);

        var exception = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(RequestWithName("magaza"), CancellationToken.None));

        Assert.Equal("This warehouse name is being used for another warehouse.", exception.Message);
        Assert.Equal("market", warehouse.WarehouseName);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_NameUnchanged_StillCallsWarehouseNameExistsButDoesNotThrow()
    {
        var warehouse = ExistingWarehouse();
        _warehouseRepository.Setup(r => r.GetWarehouse(_warehouseId)).ReturnsAsync(warehouse);
        _locationRepository.Setup(r => r.IsAddressValid(It.IsAny<Address>())).ReturnsAsync(true);
        _warehouseRepository.Setup(r => r.WarehouseNameExists("MARKET")).ReturnsAsync(true);
        _locationRepository.Setup(r => r.ConvertString(It.IsAny<Address>())).ReturnsAsync("Yeni Cadde No: 2, 34100");

        var response = await _handler.Handle(RequestWithName("market"), CancellationToken.None);

        _warehouseRepository.Verify(r => r.WarehouseNameExists("MARKET"), Times.Once);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Once);
        Assert.Equal("market", response.WarehouseName);
    }
}
