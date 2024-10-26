using FluentValidation;
using Logitar.Identity.Domain.Shared;
using Logitar.Portal.Contracts.Messages;
using Logitar.Portal.Contracts.Passwords;
using Logitar.Portal.Contracts.Tokens;
using Logitar.Portal.Contracts.Users;
using MediatR;
using SkillCraft.Application.Accounts.Constants;
using SkillCraft.Application.Accounts.Validators;
using SkillCraft.Contracts.Accounts;

namespace SkillCraft.Application.Accounts.Commands;

public record VerifyPhoneCommand(VerifyPhonePayload Payload) : Activity, IRequest<VerifyPhoneResult>;

internal class VerifyPhoneCommandHandler : IRequestHandler<VerifyPhoneCommand, VerifyPhoneResult>
{
  private readonly IMessageService _messageService;
  private readonly IOneTimePasswordService _oneTimePasswordService;
  private readonly ITokenService _tokenService;
  private readonly IUserService _userService;

  public VerifyPhoneCommandHandler(
    IMessageService messageService,
    IOneTimePasswordService oneTimePasswordService,
    ITokenService tokenService,
    IUserService userService)
  {
    _messageService = messageService;
    _oneTimePasswordService = oneTimePasswordService;
    _tokenService = tokenService;
    _userService = userService;
  }

  public async Task<VerifyPhoneResult> Handle(VerifyPhoneCommand command, CancellationToken cancellationToken)
  {
    VerifyPhonePayload payload = command.Payload;
    new VerifyPhoneValidator().ValidateAndThrow(payload);

    LocaleUnit locale = new(payload.Locale);
    User user = await FindUserAsync(payload.ProfileCompletionToken, cancellationToken);

    if (payload.Phone != null)
    {
      return await HandlePhoneAsync(payload.Phone, locale, user, cancellationToken);
    }
    else if (payload.OneTimePassword != null)
    {
      return await HandleOneTimePasswordAsync(payload.OneTimePassword, payload.ProfileCompletionToken, user, cancellationToken);
    }

    throw new ArgumentException($"Exactly one of the following must be specified: {nameof(payload.Phone)}, {nameof(payload.OneTimePassword)}.", nameof(command));
  }

  private async Task<User> FindUserAsync(string profileCompletionToken, CancellationToken cancellationToken)
  {
    ValidatedToken validatedToken = await _tokenService.ValidateAsync(profileCompletionToken, consume: false, TokenTypes.Profile, cancellationToken);
    Guid userId = validatedToken.GetUserId();
    return await _userService.FindAsync(userId, cancellationToken) ?? throw new ArgumentException($"The user 'Id={userId}' could not be found.", nameof(profileCompletionToken));
  }

  private async Task<VerifyPhoneResult> HandlePhoneAsync(AccountPhone accountPhone, LocaleUnit locale, User user, CancellationToken cancellationToken)
  {
    Phone phone = accountPhone.ToPhone();
    OneTimePassword oneTimePassword = await _oneTimePasswordService.CreateAsync(user, phone, Purposes.ContactVerification, cancellationToken);
    if (oneTimePassword.Password == null)
    {
      throw new InvalidOperationException($"The One-Time Password (OTP) 'Id={oneTimePassword.Id}' has no password.");
    }
    Dictionary<string, string> variables = new()
    {
      [Variables.OneTimePassword] = oneTimePassword.Password
    };
    string template = Templates.GetContactVerification(ContactType.Phone);
    SentMessages sentMessages = await _messageService.SendAsync(template, phone, locale, variables, cancellationToken);
    SentMessage sentMessage = sentMessages.ToSentMessage(phone);
    OneTimePasswordValidation validation = new(oneTimePassword, sentMessage);
    return new VerifyPhoneResult(validation);
  }

  private async Task<VerifyPhoneResult> HandleOneTimePasswordAsync(OneTimePasswordPayload payload, string profileCompletionToken, User user, CancellationToken cancellationToken)
  {
    OneTimePassword oneTimePassword = await _oneTimePasswordService.ValidateAsync(payload, Purposes.ContactVerification, cancellationToken);
    if (oneTimePassword.GetUserId() != user.Id)
    {
      throw new InvalidOneTimePasswordUserException(oneTimePassword, user);
    }
    Phone phone = oneTimePassword.GetPhone();
    phone.IsVerified = true;
    await InvalidateTokenAsync(profileCompletionToken, TokenTypes.Profile, cancellationToken);
    CreatedToken profileCompletion = await _tokenService.CreateAsync(user, phone, TokenTypes.Profile, cancellationToken);
    return new VerifyPhoneResult(profileCompletion);
  }
  private async Task InvalidateTokenAsync(string token, string type, CancellationToken cancellationToken)
  {
    try
    {
      _ = await _tokenService.ValidateAsync(token, consume: true, type, cancellationToken);
    }
    catch (Exception)
    {
    }
  }
}
