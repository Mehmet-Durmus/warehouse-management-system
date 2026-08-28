using MediatR;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Application.Common.Filtering.Filters;
using WHMS.Domain.BusinessRules.Warehouse;

namespace WHMS.Application.Features.Command.Warehouse.DeleteWarehouse;

public class DeleteWarehouseCommandHandler : IRequestHandler<DeleteWarehouseCommandRequest>
{
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDeliveryRepository _deliveryRepository;
    private readonly IEmployeeRepository _employeeRepository;

    public DeleteWarehouseCommandHandler(IWarehouseRepository warehouseRepository, IUnitOfWork unitOfWork, IDeliveryRepository deliveryRepository, IEmployeeRepository employeeRepository)
    {
        _warehouseRepository = warehouseRepository;
        _unitOfWork = unitOfWork;
        _deliveryRepository = deliveryRepository;
        _employeeRepository = employeeRepository;
    }

    public async Task Handle(DeleteWarehouseCommandRequest request, CancellationToken cancellationToken)
    {
        DeliveryFilter filter = new() { WarehouseId = request.WarehouseId, IsReceived = false };
        int count = await _deliveryRepository.GetDeliveriesCount(filter);
        WarehouseRules.EnsureCanBeDeleted(count > 0);
        
        var employees = await _employeeRepository.GetEmployeesByWarehouse(Guid.Parse(request.WarehouseId));
        foreach (var employee in employees)
            employee.WarehouseId = null;

        await _warehouseRepository.SoftDelete(Guid.Parse(request.WarehouseId));
        await _unitOfWork.CommitAsync();
        
    }
}