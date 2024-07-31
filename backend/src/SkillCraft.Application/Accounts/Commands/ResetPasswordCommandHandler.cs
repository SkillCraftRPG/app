using Logitar.Identity.Domain.Shared;
using Logitar.Portal.Contracts.Messages;
using Logitar.Portal.Contracts.Tokens;
using Logitar.Portal.Contracts.Users;
using MediatR;
using SkillCraft.Application.Accounts.Constants;
using SkillCraft.Contracts.Accounts;

namespace SkillCraft.Application.Accounts.Commands;

internal class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, ResetPasswordResult>
{
  private readonly IMessageService _messageService;
  private readonly ITokenService _tokenService;
  private readonly IUserService _userService;

  public ResetPasswordCommandHandler(IMessageService messageService, ITokenService tokenService, IUserService userService)
  {
    _messageService = messageService;
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

    throw new ArgumentException($"Exactly one of the following must be specified: {nameof(payload.EmailAddress)}.", nameof(command));
  }

  private async Task<ResetPasswordResult> HandleEmailAddressAsync(string emailAddress, LocaleUnit locale, CancellationToken cancellationToken)
  {
    SentMessages sentMessages;

    User? user = await _userService.FindAsync(emailAddress, cancellationToken);
    if (user == null)
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
}
