using Moq;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Application.Features.Command.Store.UpdateStore;
using WHMS.Domain.ValueObjects;

namespace WHMS.Tests.Application.Features.Command.Store.UpdateStore;

public class UpdateStoreCommandHandlerTests
{
    private readonly Mock<IStoreRepository> _storeRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<ILocationRepository> _locationRepository = new();
    private readonly UpdateStoreCommandHandler _handler;

    private readonly Guid _storeId = Guid.NewGuid();
    private readonly Guid _cityId = Guid.NewGuid();
    private readonly Guid _districtId = Guid.NewGuid();
    private readonly Guid _neighborhoodId = Guid.NewGuid();

    public UpdateStoreCommandHandlerTests()
    {
        _handler = new UpdateStoreCommandHandler(_storeRepository.Object, _locationRepository.Object, _unitOfWork.Object);
    }

    private WHMS.Domain.Entities.Store ExistingStore() => new()
    {
        Id = _storeId,
        StoreName = "market",
        NormalizedName = "MARKET",
        Address = new Address(_cityId, _districtId, _neighborhoodId, "34000", "Eski Cadde No: 1")
    };

    private UpdateStoreCommandRequest RequestWithName(string storeName) => new()
    {
        StoreId = _storeId.ToString(),
        StoreName = storeName,
        CityId = _cityId.ToString(),
        DistrictId = _districtId.ToString(),
        NeighborhoodId = _neighborhoodId.ToString(),
        AddressLine = "Yeni Cadde No: 2",
        PostalCode = "34100"
    };

    [Fact]
    public async Task Handle_StoreNotFound_ThrowsAndDoesNotCommit()
    {
        _storeRepository.Setup(r => r.GetStore(_storeId)).ReturnsAsync((WHMS.Domain.Entities.Store)null!);

        var exception = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(RequestWithName("market"), CancellationToken.None));

        Assert.Equal("Store not found.", exception.Message);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_AddressInvalid_ThrowsWithoutCheckingNameOrCommitting()
    {
        _storeRepository.Setup(r => r.GetStore(_storeId)).ReturnsAsync(ExistingStore());
        _locationRepository.Setup(r => r.IsAddressValid(It.IsAny<Address>())).ReturnsAsync(false);

        var exception = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(RequestWithName("market"), CancellationToken.None));

        Assert.Equal("Address data is invalid.", exception.Message);
        _storeRepository.Verify(r => r.StoreNameExists(It.IsAny<string>()), Times.Never);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_NameChangedAndUnique_UpdatesStoreAndCommits()
    {
        var store = ExistingStore();
        _storeRepository.Setup(r => r.GetStore(_storeId)).ReturnsAsync(store);
        _locationRepository.Setup(r => r.IsAddressValid(It.IsAny<Address>())).ReturnsAsync(true);
        _storeRepository.Setup(r => r.StoreNameExists("MAGAZA")).ReturnsAsync(false);
        _locationRepository.Setup(r => r.ConvertString(It.IsAny<Address>())).ReturnsAsync("Yeni Cadde No: 2, 34100");

        var response = await _handler.Handle(RequestWithName("magaza"), CancellationToken.None);

        Assert.Equal("magaza", store.StoreName);
        Assert.Equal("MAGAZA", store.NormalizedName);
        Assert.Equal(_neighborhoodId, store.Address.NeighborhoodId);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Once);
        Assert.Equal("magaza", response.StoreName);
        Assert.Equal("Yeni Cadde No: 2, 34100", response.Address);
    }

    [Fact]
    public async Task Handle_NameChangedButAlreadyUsedByAnotherStore_ThrowsAndLeavesStoreUnchanged()
    {
        var store = ExistingStore();
        _storeRepository.Setup(r => r.GetStore(_storeId)).ReturnsAsync(store);
        _locationRepository.Setup(r => r.IsAddressValid(It.IsAny<Address>())).ReturnsAsync(true);
        _storeRepository.Setup(r => r.StoreNameExists("MAGAZA")).ReturnsAsync(true);

        var exception = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(RequestWithName("magaza"), CancellationToken.None));

        // Note: unlike CreateStore's equivalent message, this one has no trailing
        // period - documenting the current, inconsistent text as-is.
        Assert.Equal("This name is being used for another store", exception.Message);
        Assert.Equal("market", store.StoreName);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_NameUnchanged_StillCallsStoreNameExistsButDoesNotThrow()
    {
        var store = ExistingStore();
        _storeRepository.Setup(r => r.GetStore(_storeId)).ReturnsAsync(store);
        _locationRepository.Setup(r => r.IsAddressValid(It.IsAny<Address>())).ReturnsAsync(true);
        // The store's own (unchanged) name would naturally be reported as "in use".
        _storeRepository.Setup(r => r.StoreNameExists("MARKET")).ReturnsAsync(true);
        _locationRepository.Setup(r => r.ConvertString(It.IsAny<Address>())).ReturnsAsync("Yeni Cadde No: 2, 34100");

        var response = await _handler.Handle(RequestWithName("market"), CancellationToken.None);

        // Unlike UpdateCategory/UpdateSku, this handler always calls StoreNameExists
        // even when the name did not change - it just ignores the result in that case.
        _storeRepository.Verify(r => r.StoreNameExists("MARKET"), Times.Once);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Once);
        Assert.Equal("market", response.StoreName);
    }
}
