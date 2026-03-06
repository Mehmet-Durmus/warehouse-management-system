using MediatR;

namespace WHMS.Application.Features.Command.Catalog.DeleteCategory;

public class DeleteCategoryCommandRequest : IRequest<DeleteCategoryCommandResponse>
{
    public string? CategoryId { get; set; }
}