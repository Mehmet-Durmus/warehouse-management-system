using MediatR;

namespace WHMS.Application.Features.Queries.Catalog.GetCategory;

public class GetCategoryQueryRequest : IRequest<GetCategoryQueryResponse>
{
    public string? CategoryId { get; set; }
}