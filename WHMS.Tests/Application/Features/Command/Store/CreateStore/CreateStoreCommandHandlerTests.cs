using Moq;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Application.Features.Command.Store.CreateStore;
using WHMS.Domain.Entities;
using WHMS.Domain.ValueObjects;

namespace WHMS.Tests.Application.Features.Command.Store.CreateStore;

public class CreateStoreCommandHandlerTests
{
    private readonly Mock<IStoreRepository> _storeRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<ILocationRepository> _locationRepository = new();
    private readonly CreateStoreCommandHandler _handler;

    private readonly Guid _cityId = Guid.NewGuid();
    private readonly Guid _districtId = Guid.NewGuid();
    private readonly Guid _neighborhoodId = Guid.NewGuid();

    public CreateStoreCommandHandlerTests()
    {
        _handler = new CreateStoreCommandHandler(_storeRepository.Object, _unitOfWork.Object, _locationRepository.Object);
    }

    private CreateStoreCommandRequest ValidRequest() => new()
    {
        StoreName = "market",
        CityId = _cityId.ToString(),
        DistrictId = _districtId.ToString(),
        NeighborhoodId = _neighborhoodId.ToString(),
        AddressLine = "Ana Cadde No: 1",
        PostalCode = "34000"
    };

    [Fact]
    public async Task Handle_AddressValidAndNameUnique_PersistsStoreAndReturnsResponse()
    {
        _locationRepository.Setup(r => r.IsAddressValid(It.IsAny<Address>())).ReturnsAsync(true);
        _storeRepository.Setup(r => r.StoreNameExists("MARKET")).ReturnsAsync(false);
        _locationRepository.Setup(r => r.ConvertString(It.IsAny<Address>())).ReturnsAsync("Ana Cadde No: 1, 34000");

        var response = await _handler.Handle(ValidRequest(), CancellationToken.None);

        _storeRepository.Verify(r => r.CreateStore(It.Is<WHMS.Domain.Entities.Store>(s =>
            s.StoreName == "market" &&
            s.NormalizedName == "MARKET" &&
            s.Address.CityId == _cityId &&
            s.Address.DistrictId == _districtId &&
            s.Address.NeighborhoodId == _neighborhoodId)), Times.Once);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Once);

        Assert.Equal("market", response.StoreName);
        Assert.Equal("Ana Cadde No: 1, 34000", response.Address);
    }

    [Fact]
    public async Task Handle_AddressInvalid_ThrowsWithoutCheckingNameOrPersisting()
    {
        _locationRepository.Setup(r => r.IsAddressValid(It.IsAny<Address>())).ReturnsAsync(false);

        var exception = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(ValidRequest(), CancellationToken.None));

        Assert.Equal("Address data is invalid.", exception.Message);
        _storeRepository.Verify(r => r.StoreNameExists(It.IsAny<string>()), Times.Never);
        _storeRepository.Verify(r => r.CreateStore(It.IsAny<WHMS.Domain.Entities.Store>()), Times.Never);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_NameAlreadyUsed_ThrowsAndDoesNotPersist()
    {
        _locationRepository.Setup(r => r.IsAddressValid(It.IsAny<Address>())).ReturnsAsync(true);
        _storeRepository.Setup(r => r.StoreNameExists("MARKET")).ReturnsAsync(true);

        var exception = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(ValidRequest(), CancellationToken.None));

        Assert.Equal("This name is being used for another store.", exception.Message);
        _storeRepository.Verify(r => r.CreateStore(It.IsAny<WHMS.Domain.Entities.Store>()), Times.Never);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Never);
    }
}
