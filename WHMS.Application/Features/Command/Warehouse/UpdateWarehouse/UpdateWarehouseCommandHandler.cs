using MediatR;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Domain.ValueObjects;

namespace WHMS.Application.Features.Command.Warehouse.UpdateWarehouse;

public class UpdateWarehouseCommandHandler : IRequestHandler<UpdateWarehouseCommandRequest, UpdateWarehouseCommandResponse>
{
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateWarehouseCommandHandler(IWarehouseRepository warehouseRepository, IUnitOfWork unitOfWork)
    {
        _warehouseRepository = warehouseRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<UpdateWarehouseCommandResponse> Handle(UpdateWarehouseCommandRequest request, CancellationToken cancellationToken)
    {
        var warehouse = await _warehouseRepository.GetWarehouse(Guid.Parse(request.WarehouseId));
        
        if (warehouse is null)
            throw new Exception("Warehouse not found.");
        // Mapping
        warehouse.Address = new Address
        {
            CityId = Guid.Parse(request.CityId),
            DistrictId = Guid.Parse(request.DistrictId),
            NeighborhoodId = Guid.Parse(request.NeighborhoodId),
            AddressLine = request.AddressLine,
            PostalCode = request.PostalCode
        };

        _warehouseRepository.UpdateWarehouse(warehouse);
        await _unitOfWork.CommitAsync();
        return new();
    }
}