using MediatR;
using WHMS.Application.Abstractions.Infrastructure;
using WHMS.Application.Abstractions.Persistence;

namespace WHMS.Application.Features.Command.Shipment.SendShipment;

public class SendShipmentCommandHandler : IRequestHandler<SendShipmentCommandRequest, SendShipmentCommandResponse>
{
    private readonly IShipmentRepository _shipmentRepository;
    private readonly IStockStateRepository _stockStateRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public SendShipmentCommandHandler(IShipmentRepository shipmentRepository, IStockStateRepository stockStateRepository, ICurrentUserService currentUserService, IUnitOfWork unitOfWork)
    {
        _shipmentRepository = shipmentRepository;
        _stockStateRepository = stockStateRepository;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async Task<SendShipmentCommandResponse> Handle(SendShipmentCommandRequest request, CancellationToken cancellationToken)
    {
        var warehouseId =  Guid.Parse(_currentUserService.WarehouseId!);
        var shipment = await _shipmentRepository.GetShipment(Guid.Parse(request.ShipmentId!));
        if (shipment is null || warehouseId != shipment.WarehouseId)
            throw new Exception("Shipment not found.");

        if (shipment.SendingDate is not null)
            throw new Exception("Shipment already sent.");
        shipment.SendingDate = DateTime.Now;
        shipment.SentById = _currentUserService.UserId;

        if (shipment.ShipmentItems is not null)
        {
            List<string> errors = [];
            int affectedRow = 0;
            foreach (var item in shipment.ShipmentItems)
            {
                affectedRow = await _stockStateRepository.SetQuantity(shipment.WarehouseId, item.SkuId, -item.Quantity);
                if (affectedRow == 0)
                    errors.Add($"Insufficient stock for {item.SKU!.SKUName}.");
            }
            if (errors.Count > 0) return new() {Errors = errors};
        }
        await _unitOfWork.CommitAsync();
        return new();
    }
}