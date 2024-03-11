using FluentValidation;
using Logitar.Identity.Contracts.Settings;
using Logitar.Portal.Contracts.Messages;
using Logitar.Portal.Contracts.Tokens;
using Logitar.Portal.Contracts.Users;
using MediatR;
using SkillCraft.Application.Accounts.Validators;
using SkillCraft.Contracts.Accounts;

namespace SkillCraft.Application.Accounts.Commands;

internal class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, Unit>
{
  private const string MessageTemplate = "PasswordRecovery";
  private const string TokenType = "reset_password+jwt";

  private readonly IMessageService _messageService;
  private readonly IRealmService _realmService;
  private readonly ITokenService _tokenService;
  private readonly IUserService _userService;

  public ResetPasswordCommandHandler(IMessageService messageService, IRealmService realmService, ITokenService tokenService, IUserService userService)
  {
    _messageService = messageService;
    _realmService = realmService;
    _tokenService = tokenService;
    _userService = userService;
  }

  public async Task<Unit> Handle(ResetPasswordCommand command, CancellationToken cancellationToken)
  {
    IUserSettings userSettings = await _realmService.GetUserSettingsAsync(cancellationToken);

    ResetPasswordPayload payload = command.Payload;
    new ResetPasswordValidator(userSettings.Password).ValidateAndThrow(payload);

    if (string.IsNullOrWhiteSpace(payload.EmailAddress))
    {
      string token = payload.Token ?? string.Empty;
      string password = payload.Password ?? string.Empty;

      ValidatedToken validatedToken = await _tokenService.ValidateAsync(token, TokenType, cancellationToken);
      if (validatedToken.Subject == null)
      {
        throw new InvalidOperationException($"The claim '{validatedToken.Subject}' is required.");
      }
      Guid userId = Guid.Parse(validatedToken.Subject);
      _ = await _userService.ResetPasswordAsync(userId, password, cancellationToken);
    }
    else
    {
      User? user = await _userService.FindAsync(payload.EmailAddress, cancellationToken);
      if (user?.Email != null)
      {
        CreatedToken createdToken = await _tokenService.CreateAsync(user.GetSubject(), TokenType, cancellationToken);
        Dictionary<string, string> variables = new()
        {
          ["Token"] = createdToken.Token
        };
        SentMessages sentMessages = await _messageService.SendAsync(MessageTemplate, user, payload.Locale, variables, cancellationToken);
        _ = sentMessages.ToSentMessage(user.Email);
      }
    }

    return Unit.Value;
  }
}
