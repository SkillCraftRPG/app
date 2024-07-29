using Logitar.Portal.Contracts.Sessions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SkillCraft.Application.Accounts;
using SkillCraft.Application.Accounts.Commands;
using SkillCraft.Authentication;
using SkillCraft.Contracts.Accounts;
using SkillCraft.Extensions;

namespace SkillCraft.Controllers;

[ApiController]
public class AccountController : ControllerBase
{
  private readonly IOpenAuthenticationService _openAuthenticationService;
  private readonly ISender _sender;
  private readonly ISessionService _sessionService; // TODO(fpion): implement

  public AccountController(IOpenAuthenticationService openAuthenticationService, ISender sender, ISessionService sessionService)
  {
    _openAuthenticationService = openAuthenticationService;
    _sender = sender;
    _sessionService = sessionService;
  }

  [HttpPost("/auth/sign/in")]
  public async Task<ActionResult<SignInAccountResponse>> SignInAsync([FromBody] SignInAccountPayload payload, CancellationToken cancellationToken)
  {
    SignInAccountResult result = await _sender.Send(new SignInAccountCommand(payload, HttpContext.GetSessionCustomAttributes()), cancellationToken);
    if (result.Session != null)
    {
      HttpContext.SignIn(result.Session);
    }

    SignInAccountResponse response = new(result);
    return Ok(response);
  }

  [HttpPost("/auth/token")]
  public async Task<ActionResult<GetTokenResponse>> GetTokenAsync([FromBody] GetTokenPayload payload, CancellationToken cancellationToken)
  {
    GetTokenResponse response;
    Session? session;
    if (!string.IsNullOrWhiteSpace(payload.RefreshToken))
    {
      response = new GetTokenResponse();
      session = await _sessionService.RenewAsync(payload.RefreshToken.Trim(), HttpContext.GetSessionCustomAttributes(), cancellationToken);
    }
    else
    {
      SignInAccountResult result = await _sender.Send(new SignInAccountCommand(payload, HttpContext.GetSessionCustomAttributes()), cancellationToken);
      response = new(result);
      session = result.Session;
    }

    if (session != null)
    {
      response.TokenResponse = _openAuthenticationService.GetTokenResponse(session);
    }

    return Ok(response);
  }
}
