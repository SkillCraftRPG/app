using Logitar.Portal.Contracts.Sessions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SkillCraft.Application.Accounts.Commands;
using SkillCraft.Authentication;
using SkillCraft.Contracts.Accounts;
using SkillCraft.Extensions;
using SkillCraft.Models.Account;

namespace SkillCraft.Controllers;

[ApiController]
[Route("account")]
public class AccountController : ControllerBase
{
  private readonly IAuthenticationService _authenticationService;
  private readonly IMediator _mediator;

  public AccountController(IAuthenticationService authenticationService, IMediator mediator)
  {
    _authenticationService = authenticationService;
    _mediator = mediator;
  }

  [HttpPost("sign/in")]
  public async Task<ActionResult<CurrentUser>> SignInAsync([FromBody] SignInPayload payload, CancellationToken cancellationToken)
  {
    Contracts.Accounts.SignInResult result = await _mediator.Send(new SignInCommand(payload, HttpContext.GetSessionCustomAttributes()), cancellationToken);
    if (result.Session != null)
    {
      Session session = result.Session;
      HttpContext.SignIn(session);
      CurrentUser currentUser = new(session);
      return Ok(currentUser);
    }

    throw new NotImplementedException(); // TODO(fpion): implement
  }

  [HttpPost("token")]
  public async Task<ActionResult<TokenResponse>> GetTokenAsync([FromBody] SignInPayload payload, CancellationToken cancellationToken)
  {
    Contracts.Accounts.SignInResult result = await _mediator.Send(new SignInCommand(payload, HttpContext.GetSessionCustomAttributes()), cancellationToken);
    if (result.Session != null)
    {
      Session session = result.Session;
      HttpContext.Response.Headers.CacheControl = "no-store";
      TokenResponse tokenResponse = _authenticationService.GetTokenResponse(session);
      return Ok(tokenResponse);
    }

    throw new NotImplementedException(); // TODO(fpion): implement
  }
}
