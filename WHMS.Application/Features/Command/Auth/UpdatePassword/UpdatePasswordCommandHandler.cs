using MediatR;
using Microsoft.AspNetCore.Identity;
using WHMS.Application.Abstractions.Infrastructure;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Domain.Entities;

namespace WHMS.Application.Features.Command.Auth.UpdatePassword;

public class UpdatePasswordCommandHandler : IRequestHandler<UpdatePasswordCommandRequest, UpdatePasswordCommandResponse>
{
    private readonly IAuthService _authService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public UpdatePasswordCommandHandler(IAuthService authService, ICurrentUserService currentUserService, IUnitOfWork unitOfWork)
    {
        _authService = authService;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async Task<UpdatePasswordCommandResponse> Handle(UpdatePasswordCommandRequest request, CancellationToken cancellationToken)
    {
        var user = await _authService.FindByIdAsync(_currentUserService.UserId.ToString()!);
        var result = await _authService.ChangePasswordAsync(user!, request.CurrentPassword!, request.Password!);
        if (result.Succeeded)
        {
            user!.PasswordChangedAt = DateTime.Now;
            user.IsPasswordChanged = true;
            await _unitOfWork.CommitAsync();
        }
        return new()
            { Errors = result.Succeeded ? null : result.Errors.Select(e => e.Description).ToList() };
    }
}