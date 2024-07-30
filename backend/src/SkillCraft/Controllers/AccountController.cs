using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Contracts.Users;
using MediatR;
using Microsoft.AspNetCore.Authorization;
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
  private readonly ISessionService _sessionService;

  public AccountController(IOpenAuthenticationService openAuthenticationService, ISender sender, ISessionService sessionService)
  {
    _openAuthenticationService = openAuthenticationService;
    _sender = sender;
    _sessionService = sessionService;
  }

  [HttpPost("/auth/sign/in")]
  public async Task<ActionResult<SignInResponse>> SignInAsync([FromBody] SignInPayload payload, CancellationToken cancellationToken)
  {
    SignInCommandResult result = await _sender.Send(new SignInCommand(payload, HttpContext.GetSessionCustomAttributes()), cancellationToken);
    if (result.Session != null)
    {
      HttpContext.SignIn(result.Session);
    }

    SignInResponse response = new(result);
    return Ok(response);
  }

  [HttpPost("/auth/sign/out")]
  [Authorize]
  public async Task<ActionResult> SignOutAsync(bool everywhere, CancellationToken cancellationToken)
  {
    if (everywhere)
    {
      User? user = HttpContext.GetUser();
      if (user != null)
      {
        await _sender.Send(SignOutCommand.User(user.Id), cancellationToken);
      }
    }
    else
    {
      Guid? sessionId = HttpContext.GetSessionId();
      if (sessionId.HasValue)
      {
        await _sender.Send(SignOutCommand.Session(sessionId.Value), cancellationToken);
      }
    }

    return NoContent();
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
      SignInCommandResult result = await _sender.Send(new SignInCommand(payload, HttpContext.GetSessionCustomAttributes()), cancellationToken);
      response = new(result);
      session = result.Session;
    }

    if (session != null)
    {
      response.TokenResponse = _openAuthenticationService.GetTokenResponse(session);
    }

    return Ok(response);
  }

  [HttpGet("/profile")]
  [Authorize] // TODO(fpion): User Policy
  public ActionResult<UserProfile> GetProfile()
  {
    User user = HttpContext.GetUser() ?? throw new InvalidOperationException("An authenticated user is required.");
    return Ok(user.ToUserProfile());
  }
}
