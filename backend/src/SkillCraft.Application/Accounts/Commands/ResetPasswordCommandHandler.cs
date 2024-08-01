using FluentValidation;
using Logitar.Identity.Domain.Shared;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Messages;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Contracts.Tokens;
using Logitar.Portal.Contracts.Users;
using MediatR;
using SkillCraft.Application.Accounts.Constants;
using SkillCraft.Application.Accounts.Validators;
using SkillCraft.Application.Actors;
using SkillCraft.Contracts.Accounts;

namespace SkillCraft.Application.Accounts.Commands;

internal class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, ResetPasswordResult>
{
  private readonly IActorService _actorService;
  private readonly IMessageService _messageService;
  private readonly IRealmService _realmService;
  private readonly ISessionService _sessionService;
  private readonly ITokenService _tokenService;
  private readonly IUserService _userService;

  public ResetPasswordCommandHandler(IActorService actorService,
    IMessageService messageService,
    IRealmService realmService,
    ISessionService sessionService,
    ITokenService tokenService,
    IUserService userService)
  {
    _actorService = actorService;
    _messageService = messageService;
    _realmService = realmService;
    _sessionService = sessionService;
    _tokenService = tokenService;
    _userService = userService;
  }

  public async Task<ResetPasswordResult> Handle(ResetPasswordCommand command, CancellationToken cancellationToken)
  {
    Realm realm = await _realmService.FindAsync(cancellationToken);

    ResetPasswordPayload payload = command.Payload;
    new ResetPasswordValidator(realm.PasswordSettings).ValidateAndThrow(payload);
    LocaleUnit locale = new(payload.Locale);

    if (!string.IsNullOrWhiteSpace(payload.EmailAddress))
    {
      return await HandleEmailAddressAsync(payload.EmailAddress.Trim(), locale, cancellationToken);
    }
    else if (payload.Reset != null)
    {
      return await HandleResetAsync(payload.Reset, command.CustomAttributes, cancellationToken);
    }

    throw new ArgumentException($"Exactly one of the following must be specified: {nameof(payload.EmailAddress)}, {nameof(payload.Reset)}.", nameof(command));
  }

  private async Task<ResetPasswordResult> HandleEmailAddressAsync(string emailAddress, LocaleUnit locale, CancellationToken cancellationToken)
  {
    SentMessages sentMessages;

    User? user = await _userService.FindAsync(emailAddress, cancellationToken);
    if (user == null || !user.HasPassword)
    {
      sentMessages = new([Guid.NewGuid()]);
    }
    else
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

  private async Task<ResetPasswordResult> HandleResetAsync(ResetPayload payload, IEnumerable<CustomAttribute> customAttributes, CancellationToken cancellationToken)
  {
    ValidatedToken validatedToken = await _tokenService.ValidateAsync(payload.Token, TokenTypes.PasswordRecovery, cancellationToken);
    Guid userId = validatedToken.GetUserId();
    User user = await _userService.FindAsync(userId, cancellationToken) ?? throw new ArgumentException($"The user 'Id={userId}' could not be found.", nameof(payload));
    user = await _userService.ResetPasswordAsync(user, payload.Password, cancellationToken);

    Session session = await _sessionService.CreateAsync(user, customAttributes, cancellationToken);
    await _actorService.SaveAsync(user, cancellationToken);
    return ResetPasswordResult.Success(session);
  }
}
