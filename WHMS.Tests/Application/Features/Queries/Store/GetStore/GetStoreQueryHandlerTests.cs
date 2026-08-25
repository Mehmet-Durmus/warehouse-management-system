using Moq;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Application.Features.Queries.Store.GetStore;
using WHMS.Domain.ValueObjects;

namespace WHMS.Tests.Application.Features.Queries.Store.GetStore;

public class GetStoreQueryHandlerTests
{
    private readonly Mock<IStoreRepository> _storeRepository = new();
    private readonly Mock<ILocationRepository> _locationRepository = new();
    private readonly GetStoreQueryHandler _handler;
    private readonly Guid _storeId = Guid.NewGuid();

    public GetStoreQueryHandlerTests()
    {
        _handler = new GetStoreQueryHandler(_storeRepository.Object, _locationRepository.Object);
    }

    [Fact]
    public async Task Handle_StoreNotFound_Throws()
    {
        _storeRepository.Setup(r => r.GetStore(_storeId)).ReturnsAsync((WHMS.Domain.Entities.Store)null!);

        var request = new GetStoreQueryRequest { StoreId = _storeId.ToString() };

        var exception = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(request, CancellationToken.None));

        Assert.Equal("Store not found.", exception.Message);
    }

    [Fact]
    public async Task Handle_StoreFound_ReturnsMappedResponse()
    {
        var address = new Address(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "34000", "Cadde");
        var store = new WHMS.Domain.Entities.Store { Id = _storeId, StoreName = "market", NormalizedName = "MARKET", Address = address };
        _storeRepository.Setup(r => r.GetStore(_storeId)).ReturnsAsync(store);
        _locationRepository.Setup(r => r.ConvertString(address)).ReturnsAsync("Cadde, 34000");

        var request = new GetStoreQueryRequest { StoreId = _storeId.ToString() };

        var response = await _handler.Handle(request, CancellationToken.None);

        Assert.Equal("market", response.StoreName);
        Assert.Equal(address.CityId.ToString(), response.CityId);
        Assert.Equal("Cadde, 34000", response.Address);
    }
}
