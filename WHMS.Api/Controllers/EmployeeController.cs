using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WHMS.Application.Features.Command.Employee.CreateEmployee;
using WHMS.Application.Features.Queries.Employee.GetManagers;

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
    [HttpGet("managers")]
    public async Task<IActionResult> GetManagers([FromQuery] GetManagersQueryRequest request)
        => Ok(await _mediator.Send(request));

    [Authorize(Policy = "LogisticDirector")]
    [HttpGet("staff")]
    public async Task<IActionResult> GetStaff()
        => Ok();

    [Authorize(Policy = "LogisticDirector")]
    [HttpGet("managers/{id}")]
    public async Task<IActionResult> GetManager()
        => Ok();

    [Authorize(Policy = "LogisticDirector")]
    [HttpGet("staff/{id}")]
    public async Task<IActionResult> GetStaffMember()
        => Ok();

    [Authorize(Policy = "LogisticDirector")]
    [HttpPost("manager")]
    public async Task<IActionResult> CreateManager(CreateManagerCommandRequest request) 
        => Ok(await _mediator.Send(request));
    
    [Authorize(Policy = "LogisticDirector")]
    [HttpPost("staff-member")]
    public async Task<IActionResult> CreateStaffMember() 
        => Ok();
    
    [Authorize(Policy = "LogisticDirector")]
    [HttpPut]
    public async Task<IActionResult> UpdateEmployee()
        => Ok();
    
    [Authorize(Policy = "LogisticDirector")]
    [HttpDelete]
    public async Task<IActionResult> DeleteEmployee()
        => Ok();
}