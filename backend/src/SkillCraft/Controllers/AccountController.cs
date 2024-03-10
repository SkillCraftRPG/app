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
using SkillCraft.Models.Account;

namespace SkillCraft.Controllers;

[ApiController]
[Route("account")]
public class AccountController : ControllerBase
{
  private readonly IAuthenticationService _authenticationService;
  private readonly IMediator _mediator;
  private readonly ISessionService _sessionService;
  private readonly IUserService _userService;

  private new User User => HttpContext.GetUser() ?? throw new InvalidOperationException("An authenticated user is required.");

  public AccountController(IAuthenticationService authenticationService, IMediator mediator, ISessionService sessionService, IUserService userService)
  {
    _authenticationService = authenticationService;
    _mediator = mediator;
    _sessionService = sessionService;
    _userService = userService;
  }

  [Authorize]
  [HttpGet("profile")]
  public ActionResult<User> GetProfile() // TODO(fpion): return type
  {
    return Ok(User);
  }

  [HttpPost("sign/in")]
  public async Task<ActionResult<SignInResponse<CurrentUser>>> SignInAsync([FromBody] SignInPayload payload, CancellationToken cancellationToken)
  {
    Contracts.Accounts.SignInResult result = await _mediator.Send(new SignInCommand(payload, HttpContext.GetSessionCustomAttributes()), cancellationToken);
    CurrentUser? currentUser = null;
    if (result.Session != null)
    {
      currentUser = new(result.Session);
      HttpContext.SignIn(result.Session);
    }
    SignInResponse<CurrentUser> response = new(result, currentUser);
    return Ok(response);
  }

  [HttpPost("token")]
  public async Task<ActionResult<SignInResponse<TokenResponse>>> GetTokenAsync([FromBody] GetTokenPayload payload, CancellationToken cancellationToken)
  {
    Contracts.Accounts.SignInResult? result = null;
    TokenResponse? token = null;
    if (string.IsNullOrWhiteSpace(payload.RefreshToken))
    {
      result = await _mediator.Send(new SignInCommand(payload, HttpContext.GetSessionCustomAttributes()), cancellationToken);
      if (result.Session != null)
      {
        token = _authenticationService.GetTokenResponse(result.Session);
      }
    }
    else
    {
      Session session = await _sessionService.RenewAsync(payload.RefreshToken.Trim(), HttpContext.GetSessionCustomAttributes(), cancellationToken);
      token = _authenticationService.GetTokenResponse(session);
    }
    SignInResponse<TokenResponse> response = new(result, token);
    return Ok(response);
  }

  [Authorize]
  [HttpPost("sign/out/all")]
  public async Task<ActionResult> SignOutAllAsync(CancellationToken cancellationToken)
  {
    _ = await _userService.SignOutAsync(User, cancellationToken);
    HttpContext.SignOut();
    return NoContent();
  }

  [Authorize]
  [HttpPost("sign/out")]
  public async Task<ActionResult> SignOutAsync(CancellationToken cancellationToken)
  {
    Session? session = HttpContext.GetSession();
    if (session != null)
    {
      _ = await _sessionService.SignOutAsync(session, cancellationToken);
    }
    HttpContext.SignOut();
    return NoContent();
  }
}
