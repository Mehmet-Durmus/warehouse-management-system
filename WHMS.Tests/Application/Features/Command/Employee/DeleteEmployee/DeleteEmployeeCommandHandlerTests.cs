using Moq;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Application.Features.Command.Employee.DeleteEmployee;
using WHMS.Domain.Entities;
using WHMS.Domain.Exceptions;

namespace WHMS.Tests.Application.Features.Command.Employee.DeleteEmployee;

public class DeleteEmployeeCommandHandlerTests
{
    private readonly Mock<IEmployeeRepository> _employeeRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly DeleteEmployeeCommandHandler _handler;
    private readonly Guid _employeeId = Guid.NewGuid();

    public DeleteEmployeeCommandHandlerTests()
    {
        _handler = new DeleteEmployeeCommandHandler(_employeeRepository.Object, _unitOfWork.Object);
    }

    private DeleteEmployeeCommandRequest Request() => new() { EmployeeId = _employeeId.ToString() };

    [Fact]
    public async Task Handle_EmployeeIsAManager_DeactivatesWithoutCheckingStaff()
    {
        var manager = new ApplicationUser { Id = _employeeId, UserName = "WHM_0001", FullName = "Test Manager", IsActive = true };
        _employeeRepository.Setup(r => r.GetManager(_employeeId)).ReturnsAsync(manager);

        await _handler.Handle(Request(), CancellationToken.None);

        Assert.False(manager.IsActive);
        _employeeRepository.Verify(r => r.GetStaffMember(It.IsAny<Guid>()), Times.Never);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_EmployeeIsAStaffMember_Deactivates()
    {
        var staff = new ApplicationUser { Id = _employeeId, UserName = "WHS_0001", FullName = "Test Staff", IsActive = true };
        _employeeRepository.Setup(r => r.GetManager(_employeeId)).ReturnsAsync((ApplicationUser)null!);
        _employeeRepository.Setup(r => r.GetStaffMember(_employeeId)).ReturnsAsync(staff);

        await _handler.Handle(Request(), CancellationToken.None);

        Assert.False(staff.IsActive);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_EmployeeNotFoundAsEitherRole_ThrowsAndDoesNotCommit()
    {
        _employeeRepository.Setup(r => r.GetManager(_employeeId)).ReturnsAsync((ApplicationUser)null!);
        _employeeRepository.Setup(r => r.GetStaffMember(_employeeId)).ReturnsAsync((ApplicationUser)null!);

        var exception = await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(Request(), CancellationToken.None));

        Assert.Equal("Employee not found.", exception.Message);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Never);
    }
}
