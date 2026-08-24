using Moq;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Application.Common.Constants;
using WHMS.Application.Features.Command.Warehouse.CreateWarehouse;
using WHMS.Domain.Entities;
using WHMS.Domain.ValueObjects;

namespace WHMS.Tests.Application.Features.Command.Warehouse.CreateWarehouse;

public class CreateWarehouseCommandHandlerTests
{
    private readonly Mock<IWarehouseRepository> _warehouseRepository = new();
    private readonly Mock<ILocationRepository> _locationRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<IEmployeeRepository> _employeeRepository = new();
    private readonly Mock<IAuthService> _authService = new();
    private readonly CreateWarehouseCommandHandler _handler;

    private readonly Guid _cityId = Guid.NewGuid();
    private readonly Guid _districtId = Guid.NewGuid();
    private readonly Guid _neighborhoodId = Guid.NewGuid();
    private readonly Guid _generatedWarehouseId = Guid.NewGuid();

    public CreateWarehouseCommandHandlerTests()
    {
        _handler = new CreateWarehouseCommandHandler(
            _warehouseRepository.Object, _unitOfWork.Object, _locationRepository.Object,
            _employeeRepository.Object, _authService.Object);
    }

    private void SetValidAddressAndUniqueName()
    {
        _locationRepository.Setup(r => r.IsAddressValid(It.IsAny<Address>())).ReturnsAsync(true);
        _warehouseRepository.Setup(r => r.WarehouseNameExists("MARKET")).ReturnsAsync(false);
        _locationRepository.Setup(r => r.ConvertString(It.IsAny<Address>())).ReturnsAsync("Cadde No: 1, 34000");
        // Mirrors EF Core's real behavior: the Guid primary key is generated the
        // moment the entity is added to the change tracker, not at SaveChanges.
        _warehouseRepository
            .Setup(r => r.CreateWarehouse(It.IsAny<WHMS.Domain.Entities.Warehouse>()))
            .Callback<WHMS.Domain.Entities.Warehouse>(w => w.Id = _generatedWarehouseId)
            .Returns(Task.CompletedTask);
    }

    private CreateWarehouseCommandRequest Request(string? managerId = null, List<string>? staffIds = null) => new()
    {
        WarehouseName = "market",
        CityId = _cityId.ToString(),
        DistrictId = _districtId.ToString(),
        NeighborhoodId = _neighborhoodId.ToString(),
        AddressLine = "Cadde No: 1",
        PostalCode = "34000",
        ManagerId = managerId,
        StaffIds = staffIds
    };

    [Fact]
    public async Task Handle_AddressInvalid_ThrowsWithoutCheckingNameOrCreating()
    {
        _locationRepository.Setup(r => r.IsAddressValid(It.IsAny<Address>())).ReturnsAsync(false);

        var exception = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(Request(), CancellationToken.None));

        Assert.Equal("Address data is invalid.", exception.Message);
        _warehouseRepository.Verify(r => r.WarehouseNameExists(It.IsAny<string>()), Times.Never);
        _warehouseRepository.Verify(r => r.CreateWarehouse(It.IsAny<WHMS.Domain.Entities.Warehouse>()), Times.Never);
    }

    [Fact]
    public async Task Handle_NameAlreadyUsed_ThrowsWithoutCreating()
    {
        _locationRepository.Setup(r => r.IsAddressValid(It.IsAny<Address>())).ReturnsAsync(true);
        _warehouseRepository.Setup(r => r.WarehouseNameExists("MARKET")).ReturnsAsync(true);

        var exception = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(Request(), CancellationToken.None));

        Assert.Equal("This warehouse name is being used for another warehouse.", exception.Message);
        _warehouseRepository.Verify(r => r.CreateWarehouse(It.IsAny<WHMS.Domain.Entities.Warehouse>()), Times.Never);
    }

    [Fact]
    public async Task Handle_NoManagerOrStaff_CreatesWarehouseAndReturnsResult()
    {
        SetValidAddressAndUniqueName();

        var response = await _handler.Handle(Request(), CancellationToken.None);

        Assert.Empty(response.Warnings);
        Assert.Equal(_generatedWarehouseId.ToString(), response.ResultDto.WarehouseId);
        Assert.Equal("market", response.ResultDto.WarehouseName);
        Assert.Equal("Cadde No: 1, 34000", response.ResultDto.Address);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_ValidManagerProvided_AssignsTheRealGeneratedWarehouseId()
    {
        SetValidAddressAndUniqueName();
        var manager = new ApplicationUser { Id = Guid.NewGuid(), UserName = "WHM_0001", FullName = "Test Manager" };
        _authService.Setup(s => s.FindByIdAsync(manager.Id.ToString())).ReturnsAsync(manager);
        _authService.Setup(s => s.GetRolesAsync(manager)).ReturnsAsync([ApplicationRole.WarehouseManager]);

        var response = await _handler.Handle(Request(managerId: manager.Id.ToString()), CancellationToken.None);

        Assert.Empty(response.Warnings);
        Assert.NotEqual(Guid.Empty, manager.WarehouseId);
        Assert.Equal(_generatedWarehouseId, manager.WarehouseId);
    }

    [Fact]
    public async Task Handle_ValidStaffProvided_AssignsTheRealGeneratedWarehouseId()
    {
        SetValidAddressAndUniqueName();
        var staff = new ApplicationUser { Id = Guid.NewGuid(), UserName = "WHS_0001", FullName = "Test Staff" };
        _authService.Setup(s => s.FindByIdAsync(staff.Id.ToString())).ReturnsAsync(staff);
        _authService.Setup(s => s.GetRolesAsync(staff)).ReturnsAsync([ApplicationRole.WarehouseStaff]);

        var response = await _handler.Handle(Request(staffIds: [staff.Id.ToString()]), CancellationToken.None);

        Assert.Empty(response.Warnings);
        Assert.Equal(_generatedWarehouseId, staff.WarehouseId);
    }

    [Fact]
    public async Task Handle_ManagerNotFound_WarnsButStillCreatesWarehouse()
    {
        SetValidAddressAndUniqueName();
        _authService.Setup(s => s.FindByIdAsync("missing")).ReturnsAsync((ApplicationUser?)null);

        var response = await _handler.Handle(Request(managerId: "missing"), CancellationToken.None);

        Assert.Equal(["'missing' is invalid. Manager not found."], response.Warnings);
        Assert.Equal(_generatedWarehouseId.ToString(), response.ResultDto.WarehouseId);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Once);
    }
}
