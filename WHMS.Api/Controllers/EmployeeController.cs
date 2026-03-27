using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WHMS.Api.Common.Models;
using WHMS.Application.Features.Command.Employee.CreateEmployee;
using WHMS.Application.Features.Command.Employee.CreateStaffMember;
using WHMS.Application.Features.Command.Employee.DeleteEmployee;
using WHMS.Application.Features.Command.Employee.UpdateManager;
using WHMS.Application.Features.Command.Employee.UpdateStaffMember;
using WHMS.Application.Features.Queries.Employee.GetAllEmployees;
using WHMS.Application.Features.Queries.Employee.GetManager;
using WHMS.Application.Features.Queries.Employee.GetManagers;
using WHMS.Application.Features.Queries.Employee.GetStaff;
using WHMS.Application.Features.Queries.Employee.GetStaffMember;

namespace WHMS.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EmployeeController : ControllerBase
{
    private readonly IMediator _mediator;

    public EmployeeController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize(Policy = "LogisticDirector")]
    [HttpGet()]
    public async Task<IActionResult> GetEmployees([FromQuery] GetAllEmployeesQueryRequest request)
    {
        var result = await _mediator.Send(request);
        return Ok(ApiResponse<List<GetAllEmployeesResultUserDto>>.SuccessList(result.Employees!, new(result.Pagination)));
    }


    [Authorize(Policy = "LogisticDirector")]
    [HttpGet("managers")]
    public async Task<IActionResult> GetManagers()
        => Ok(await _mediator.Send(new GetManagersQueryRequest()));

    [Authorize(Policy = "LogisticDirector")]
    [HttpGet("staff")]
    public async Task<IActionResult> GetStaff()
        => Ok(await _mediator.Send(new GetStaffQueryRequest()));

    [Authorize(Policy = "LogisticDirector")]
    [HttpGet("managers/{id}")]
    public async Task<IActionResult> GetManager(string id)
        => Ok(await _mediator.Send(new GetManagerQueryRequest {ManagerId = id}));

    [Authorize(Policy = "LogisticDirector")]
    [HttpGet("staff/{id}")]
    public async Task<IActionResult> GetStaffMember(string id)
        => Ok(await _mediator.Send(new GetStaffMemberQueryRequest {StaffMemberId = id}));

    [Authorize(Policy = "LogisticDirector")]
    [HttpPost("manager")]
    public async Task<IActionResult> CreateManager(CreateManagerCommandRequest request) 
        => Ok(ApiResponse<CreateManagerCommandResponse>.Success(await _mediator.Send(request)));
    
    [Authorize(Policy = "LogisticDirector")]
    [HttpPost("staff-member")]
    public async Task<IActionResult> CreateStaffMember(CreateStaffMemberCommandRequest request) 
        => Ok(ApiResponse<CreateStaffMemberCommandResponse>.Success(await _mediator.Send(request)));
    
    [Authorize(Policy = "LogisticDirector")]
    [HttpPut("manager")]
    public async Task<IActionResult> UpdateManager(UpdateManagerCommandRequest request)
        => Ok(ApiResponse<UpdateManagerCommandResponse>.Success(await _mediator.Send(request)));
    
    [Authorize(Policy = "LogisticDirector")]
    [HttpPut("staff-member")]
    public async Task<IActionResult> UpdateStaffMember(UpdateStaffMemberCommandRequest request)
        => Ok(ApiResponse<UpdateStaffMemberCommandResponse>.Success(await _mediator.Send(request)));
    
    [Authorize(Policy = "LogisticDirector")]
    [HttpDelete]
    public async Task<IActionResult> DeleteEmployee(DeleteEmployeeCommandRequest request)
    {
        await _mediator.Send(request);
        return NoContent();
    }
}