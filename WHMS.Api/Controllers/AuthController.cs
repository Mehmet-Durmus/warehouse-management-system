using System.Reflection.Metadata.Ecma335;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Update.Internal;
using WHMS.Api.Common.Models;
using WHMS.Application.Features.Command.Auth.Login;
using WHMS.Application.Features.Command.Auth.UpdatePassword;
using WHMS.Application.Features.Queries.CurrentUserInfo;

namespace WHMS.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginCommandRequest request)
        => Ok(await _mediator.Send(request));

    [Authorize(Policy = "DirectorManagerOrStaff")]
    [HttpGet("isloggedin")]
    public async Task<IActionResult> IsLoggedin() 
        => Ok(await _mediator.Send(new CurrentUserInfoQueryRequest()));

    [Authorize(Policy = "PasswordChange")]
    [HttpPut("update-password")]
    public async Task<IActionResult> UpdatePassword(UpdatePasswordCommandRequest request)
    {
        var result = await _mediator.Send(request);
        if (result.Errors is not null || result.Errors?.Count > 0)
            return BadRequest(ApiResponse.Failure(result.Errors!));
        return NoContent();
    }
}