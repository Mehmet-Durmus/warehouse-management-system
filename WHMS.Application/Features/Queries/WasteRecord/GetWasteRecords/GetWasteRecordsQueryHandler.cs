using MediatR;
using WHMS.Application.Abstractions.Infrastructure;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Application.Common.Constants;
using WHMS.Application.Common.Filtering.Filters;

namespace WHMS.Application.Features.Queries.WasteRecord.GetWasteRecords;

public class GetWasteRecordsQueryHandler : IRequestHandler<GetWasteRecordsQueryRequest, GetWasteRecordsQueryResponse>
{
    private readonly IWasteRecordRepository _wasteRecordRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IEmployeeRepository _employeeRepository;

    public GetWasteRecordsQueryHandler(IWasteRecordRepository wasteRecordRepository, ICurrentUserService currentUserService, IEmployeeRepository employeeRepository)
    {
        _wasteRecordRepository = wasteRecordRepository;
        _currentUserService = currentUserService;
        _employeeRepository = employeeRepository;
    }

    public async Task<GetWasteRecordsQueryResponse> Handle(GetWasteRecordsQueryRequest request, CancellationToken cancellationToken)
    {
        WasteRecordFilter filter = new()
        {
            WarehouseId = request.WarehouseId,
            SkuId = request.SkuId,
            CreatedById = request.CreatedById,
            MinQuantity = request.MinQuantity,
            MaxQuantity = request.MaxQuantity,
            Page = request.Page,
            PageSize = request.PageSize
        };

        var roles = _currentUserService.Roles;
        if (request.CreatedById is not null)
        {
            var employee = await _employeeRepository.GetEmployee(Guid.Parse(request.CreatedById!));
            if (roles!.Contains(ApplicationRole.WarehouseManager) && employee.WarehouseId.ToString() != _currentUserService.WarehouseId)
                throw new Exception("Employee not found.");
        }

        if (roles!.Contains(ApplicationRole.WarehouseManager))
            filter.WarehouseId = _currentUserService.WarehouseId;
        else if (roles!.Contains(ApplicationRole.WarehouseStaff))
        {
            filter.WarehouseId = _currentUserService.WarehouseId;
            filter.CreatedById = _currentUserService.UserId.ToString();
        }

        int count = await _wasteRecordRepository.GetWasteRecordCount(filter);
        var wasteRecords = await _wasteRecordRepository.GetWasteRecords(filter, applyPagination: true);

        GetWasteRecordsQueryResponse response = new() { WasteRecords = [] };
        response.Pagination = new(
            request.Page,
            request.PageSize,
            (int)Math.Ceiling((double) count / request.PageSize)
        );

        foreach (var wasteRecord in wasteRecords)
            response.WasteRecords.Add(new()
            {
                WasteRecordId = wasteRecord.Id.ToString(),
                WarehouseId = wasteRecord.WarehouseId.ToString(),
                SkuId = wasteRecord.Sku!.Id.ToString(),
                SkuName = wasteRecord.Sku.SKUName,
                Quantity = wasteRecord.Quantity,
                Description = wasteRecord.Description,
                CreatedAt = wasteRecord.CreatedAt,
                CreatedById = wasteRecord.CreatedById.ToString()!,
                CreatedByName = wasteRecord.CreatedByName ?? "",
                CreatedByUsername = wasteRecord.CreatedByUserName ?? ""
            });
        return response;
    }
}