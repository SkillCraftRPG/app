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
  public async Task<ActionResult<SignInResponse<TokenResponse>>> GetTokenAsync([FromBody] SignInPayload payload, CancellationToken cancellationToken)
  {
    Contracts.Accounts.SignInResult result = await _mediator.Send(new SignInCommand(payload, HttpContext.GetSessionCustomAttributes()), cancellationToken);
    TokenResponse? token = result.Session == null ? null : _authenticationService.GetTokenResponse(result.Session);
    SignInResponse<TokenResponse> response = new(result, token);
    return Ok(response);
  }
}
