using MediatR;
using WHMS.Application.Abstractions.Infrastructure;

namespace WHMS.Application.Features.Queries.CurrentUserInfo;

public class CurrentUserInfoQueryHandler : IRequestHandler<CurrentUserInfoQueryRequest, CurrentUserInfoQueryResponse>
{
    private readonly ICurrentUserService _currentUserService;

    public CurrentUserInfoQueryHandler(ICurrentUserService currentUserService)
    {
        _currentUserService = currentUserService;
    }

    public async Task<CurrentUserInfoQueryResponse> Handle(CurrentUserInfoQueryRequest request, CancellationToken cancellationToken)
    {
        return new()
        {
            WarehouseId = _currentUserService.WarehouseId,
            UserName = _currentUserService.UserName!,
            FullName = _currentUserService.FullName!
        };
    }
}