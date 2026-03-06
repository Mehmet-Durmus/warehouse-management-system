using MediatR;

namespace WHMS.Application.Features.Command.Catalog.UpdateCategory;

public class UpdateCategoryCommandRequest : IRequest<UpdateCategoryCommandResponse>
{
    public string? CategoryId { get; set; }
    public string? CategoryName { get; set; }
}