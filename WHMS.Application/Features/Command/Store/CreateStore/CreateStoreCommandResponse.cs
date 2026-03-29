namespace WHMS.Application.Features.Command.Store.CreateStore;

public class CreateStoreCommandResponse
{
    public string StoreId { get; set; } = null!;
    public string StoreName { get; set; } = null!;
    public string Address { get; set; } = null!;
}