using System.Reflection.Metadata.Ecma335;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WHMS.Application.Features.Command.Login;
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

}