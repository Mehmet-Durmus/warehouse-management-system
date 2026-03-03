using MediatR;

namespace WHMS.Application.Features.Command.Catalog;

public class CreateCategoryCommandRequest : IRequest<CreateCategoryCommandResponse>
{
    public string? CategoryName { get; set; }
}