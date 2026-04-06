using MediatR;
using WHMS.Application.Abstractions.Infrastructure;
using WHMS.Application.Abstractions.Persistence;

namespace WHMS.Application.Features.Command.Employee.UpdateEmployeePassword;

public class UpdateEmployeePasswordCommandHandler : IRequestHandler<UpdateEmployeePasswordCommandRequest, UpdateEmployeePasswordCommandResponse>
{
    private readonly IAuthService _authService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IPasswordCreator _passwordCreator;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateEmployeePasswordCommandHandler(IAuthService authService, ICurrentUserService currentUserService, IPasswordCreator passwordCreator, IUnitOfWork unitOfWork)
    {
        _authService = authService;
        _currentUserService = currentUserService;
        _passwordCreator = passwordCreator;
        _unitOfWork = unitOfWork;
    }

    public async Task<UpdateEmployeePasswordCommandResponse> Handle(UpdateEmployeePasswordCommandRequest request, CancellationToken cancellationToken)
    {
        var employee = await _authService.FindByIdAsync(request.EmployeeId!);
        if (employee is null || employee.Id == _currentUserService.UserId)
            throw new Exception("Employee not found.");
            
        string tempPassword = await _passwordCreator.CreateTempPassword();
        var token = await _authService.GeneratePasswordResetTokenAsync(employee);
        var result = await _authService.ResetPasswordAsync(employee, token, tempPassword);

        UpdateEmployeePasswordCommandResponse response = new();

        if (result.Succeeded)
        {
            employee.IsPasswordChanged = false;
            employee.PasswordChangedAt = DateTime.Now;
            await _unitOfWork.CommitAsync();
            response.Result = new();
            response.Result.EmployeeId = employee.Id.ToString();
            response.Result.EmployeeUserName = employee.UserName!;
            response.Result.EmployeeFullName = employee.FullName;
            response.Result.TempPassword = tempPassword;
        }
        else
            response.Errors = result.Succeeded ? null : result.Errors.Select(e => e.Description).ToList();

        return response;
    }
}