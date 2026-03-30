using MediatR;

namespace WHMS.Application.Features.Queries.CurrentUserInfo;

public class CurrentUserInfoQueryRequest : IRequest<CurrentUserInfoQueryResponse> {}