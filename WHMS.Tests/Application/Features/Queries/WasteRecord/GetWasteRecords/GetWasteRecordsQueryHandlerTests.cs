using Moq;
using WHMS.Application.Abstractions.Infrastructure;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Application.Common.Constants;
using WHMS.Application.Common.Filtering.Filters;
using WHMS.Application.Features.Queries.WasteRecord.GetWasteRecords;
using WHMS.Domain.Entities;

namespace WHMS.Tests.Application.Features.Queries.WasteRecord.GetWasteRecords;

public class GetWasteRecordsQueryHandlerTests
{
    private readonly Mock<IWasteRecordRepository> _wasteRecordRepository = new();
    private readonly Mock<ICurrentUserService> _currentUserService = new();
    private readonly Mock<IEmployeeRepository> _employeeRepository = new();
    private readonly GetWasteRecordsQueryHandler _handler;
    private readonly Guid _warehouseId = Guid.NewGuid();
    private readonly Guid _userId = Guid.NewGuid();

    public GetWasteRecordsQueryHandlerTests()
    {
        _currentUserService.Setup(u => u.UserId).Returns(_userId);
        _wasteRecordRepository.Setup(r => r.GetWasteRecordCount(It.IsAny<WasteRecordFilter>())).ReturnsAsync(0);
        _wasteRecordRepository.Setup(r => r.GetWasteRecords(It.IsAny<WasteRecordFilter>(), true)).ReturnsAsync([]);
        _handler = new GetWasteRecordsQueryHandler(_wasteRecordRepository.Object, _currentUserService.Object, _employeeRepository.Object);
    }

    private void SetRoles(params string[] roles) => _currentUserService.Setup(u => u.Roles).Returns(roles.ToList());
    private GetWasteRecordsQueryRequest Request(string? warehouseId = null, string? createdById = null) => new()
    {
        WarehouseId = warehouseId, CreatedById = createdById, Page = 1, PageSize = 20
    };

    [Fact]
    public async Task Handle_Director_UsesRequestedWarehouseAsIs()
    {
        SetRoles(ApplicationRole.LogisticDirector);
        WasteRecordFilter? captured = null;
        _wasteRecordRepository
            .Setup(r => r.GetWasteRecords(It.IsAny<WasteRecordFilter>(), true))
            .Callback<WasteRecordFilter, bool>((f, _) => captured = f)
            .ReturnsAsync([]);

        await _handler.Handle(Request(warehouseId: "requested-warehouse"), CancellationToken.None);

        Assert.Equal("requested-warehouse", captured!.WarehouseId);
    }

    [Fact]
    public async Task Handle_Manager_ForcesOwnWarehouseRegardlessOfRequest()
    {
        SetRoles(ApplicationRole.WarehouseManager);
        _currentUserService.Setup(u => u.WarehouseId).Returns(_warehouseId.ToString());
        WasteRecordFilter? captured = null;
        _wasteRecordRepository
            .Setup(r => r.GetWasteRecords(It.IsAny<WasteRecordFilter>(), true))
            .Callback<WasteRecordFilter, bool>((f, _) => captured = f)
            .ReturnsAsync([]);

        await _handler.Handle(Request(warehouseId: "requested-warehouse"), CancellationToken.None);

        Assert.Equal(_warehouseId.ToString(), captured!.WarehouseId);
    }

    [Fact]
    public async Task Handle_Staff_ForcesOwnWarehouseAndOwnCreatedByRegardlessOfRequest()
    {
        SetRoles(ApplicationRole.WarehouseStaff);
        _currentUserService.Setup(u => u.WarehouseId).Returns(_warehouseId.ToString());
        WasteRecordFilter? captured = null;
        _wasteRecordRepository
            .Setup(r => r.GetWasteRecords(It.IsAny<WasteRecordFilter>(), true))
            .Callback<WasteRecordFilter, bool>((f, _) => captured = f)
            .ReturnsAsync([]);

        await _handler.Handle(Request(warehouseId: "requested-warehouse", createdById: Guid.NewGuid().ToString()), CancellationToken.None);

        Assert.Equal(_warehouseId.ToString(), captured!.WarehouseId);
        Assert.Equal(_userId.ToString(), captured.CreatedById);
    }

    [Fact]
    public async Task Handle_ManagerFilteringByCreatorInAnotherWarehouse_Throws()
    {
        SetRoles(ApplicationRole.WarehouseManager);
        _currentUserService.Setup(u => u.WarehouseId).Returns(_warehouseId.ToString());
        var creatorId = Guid.NewGuid();
        var creator = new ApplicationUser { Id = creatorId, UserName = "WHS_0001", FullName = "Staff", WarehouseId = Guid.NewGuid() };
        _employeeRepository.Setup(r => r.GetEmployee(creatorId)).ReturnsAsync(creator);

        var exception = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(Request(createdById: creatorId.ToString()), CancellationToken.None));

        Assert.Equal("Employee not found.", exception.Message);
    }

    [Fact]
    public async Task Handle_ManagerFilteringByCreatorInOwnWarehouse_Succeeds()
    {
        SetRoles(ApplicationRole.WarehouseManager);
        _currentUserService.Setup(u => u.WarehouseId).Returns(_warehouseId.ToString());
        var creatorId = Guid.NewGuid();
        var creator = new ApplicationUser { Id = creatorId, UserName = "WHS_0001", FullName = "Staff", WarehouseId = _warehouseId };
        _employeeRepository.Setup(r => r.GetEmployee(creatorId)).ReturnsAsync(creator);

        var response = await _handler.Handle(Request(createdById: creatorId.ToString()), CancellationToken.None);

        Assert.NotNull(response.Pagination);
    }

    [Fact]
    public async Task Handle_ManagerFilteringByNonExistentCreator_ThrowsNotFound()
    {
        SetRoles(ApplicationRole.WarehouseManager);
        _currentUserService.Setup(u => u.WarehouseId).Returns(_warehouseId.ToString());
        var missingId = Guid.NewGuid();
        _employeeRepository.Setup(r => r.GetEmployee(missingId)).ReturnsAsync((ApplicationUser)null!);

        var exception = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(Request(createdById: missingId.ToString()), CancellationToken.None));

        Assert.Equal("Employee not found.", exception.Message);
    }

    [Fact]
    public async Task Handle_DirectorFilteringByNonExistentCreator_DoesNotThrow()
    {
        // The manager-only check is skipped entirely for directors, so a
        // non-existent CreatedById does not trigger the guard above at all.
        SetRoles(ApplicationRole.LogisticDirector);
        var missingId = Guid.NewGuid();
        _employeeRepository.Setup(r => r.GetEmployee(missingId)).ReturnsAsync((ApplicationUser)null!);

        var response = await _handler.Handle(Request(createdById: missingId.ToString()), CancellationToken.None);

        Assert.NotNull(response.Pagination);
    }
}
