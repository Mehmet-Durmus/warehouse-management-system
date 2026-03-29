using MediatR;

namespace WHMS.Application.Features.Command.Store.DeleteStore;

public class DeleteStoreCommandRequest : IRequest
{
    public string? StoreId { get; set; }
}