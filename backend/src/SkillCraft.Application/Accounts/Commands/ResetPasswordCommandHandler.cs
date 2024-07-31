using Logitar.Identity.Domain.Shared;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Messages;
using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Contracts.Tokens;
using Logitar.Portal.Contracts.Users;
using MediatR;
using SkillCraft.Application.Accounts.Constants;
using SkillCraft.Application.Actors;
using SkillCraft.Contracts.Accounts;

namespace SkillCraft.Application.Accounts.Commands;

internal class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, ResetPasswordResult>
{
  private readonly IActorService _actorService;
  private readonly IMessageService _messageService;
  private readonly ISessionService _sessionService;
  private readonly ITokenService _tokenService;
  private readonly IUserService _userService;

  public ResetPasswordCommandHandler(IActorService actorService,
    IMessageService messageService,
    ISessionService sessionService,
    ITokenService tokenService,
    IUserService userService)
  {
    _actorService = actorService;
    _messageService = messageService;
    _sessionService = sessionService;
    _tokenService = tokenService;
    _userService = userService;
  }

  public async Task<ResetPasswordResult> Handle(ResetPasswordCommand command, CancellationToken cancellationToken)
  {
    ResetPasswordPayload payload = command.Payload;
    // TODO(fpion): validate payload
    LocaleUnit locale = new(payload.Locale);

    if (!string.IsNullOrWhiteSpace(payload.EmailAddress))
    {
      return await HandleEmailAddressAsync(payload.EmailAddress.Trim(), locale, cancellationToken);
    }
    else if (!string.IsNullOrEmpty(payload.Token) && !string.IsNullOrWhiteSpace(payload.Password))
    {
      return await HandleNewPasswordAsync(payload.Token.Trim(), payload.Password, command.CustomAttributes, cancellationToken);
    }

    throw new ArgumentException($"Exactly one of the following must be specified: {nameof(payload.EmailAddress)}.", nameof(command)); // TODO(fpion): implement
  }

  private async Task<ResetPasswordResult> HandleEmailAddressAsync(string emailAddress, LocaleUnit locale, CancellationToken cancellationToken)
  {
    SentMessages sentMessages;

    User? user = await _userService.FindAsync(emailAddress, cancellationToken);
    if (user == null)
    {
      sentMessages = new([Guid.NewGuid()]);
    }
    else // TODO(fpion): can we reset the password of an user without any password?
    {
      CreatedToken passwordRecovery = await _tokenService.CreateAsync(user, TokenTypes.PasswordRecovery, cancellationToken);
      Dictionary<string, string> variables = new()
      {
        [Variables.Token] = passwordRecovery.Token
      };
      sentMessages = await _messageService.SendAsync(Templates.PasswordRecovery, user, ContactType.Email, locale, variables, cancellationToken);
    }

    Email email = new(emailAddress);
    SentMessage sentMessage = sentMessages.ToSentMessage(email);
    return ResetPasswordResult.RecoveryLinkSent(sentMessage);
  }

  private async Task<ResetPasswordResult> HandleNewPasswordAsync(string token, string password, IEnumerable<CustomAttribute> customAttributes, CancellationToken cancellationToken) // TODO(fpion): rename
  {
    ValidatedToken validatedToken = await _tokenService.ValidateAsync(token, TokenTypes.PasswordRecovery, cancellationToken);
    Guid userId = validatedToken.GetUserId();
    User user = await _userService.FindAsync(userId, cancellationToken) ?? throw new ArgumentException($"The user 'Id={userId}' could not be found.", nameof(token));
    user = await _userService.ResetPasswordAsync(user, password, cancellationToken);

    return await EnsureProfileIsCompletedAsync(user, customAttributes, cancellationToken);
  }

  private async Task<ResetPasswordResult> EnsureProfileIsCompletedAsync(User user, IEnumerable<CustomAttribute> customAttributes, CancellationToken cancellationToken)
  {
    if (!user.IsProfileCompleted())
    {
      CreatedToken profileCompletion = await _tokenService.CreateAsync(user, TokenTypes.Profile, cancellationToken);
      return ResetPasswordResult.RequireProfileCompletion(profileCompletion);
    }

    Session session = await _sessionService.CreateAsync(user, customAttributes, cancellationToken);
    await _actorService.SaveAsync(user, cancellationToken); // TODO(fpion): is it really necessary?
    return ResetPasswordResult.Success(session);
  } // TODO(fpion): refactor
}
