using Logitar.Identity.Domain.Shared;
using Logitar.Portal.Contracts.Messages;
using Logitar.Portal.Contracts.Passwords;
using Logitar.Portal.Contracts.Tokens;
using Logitar.Portal.Contracts.Users;
using MediatR;
using SkillCraft.Application.Accounts.Constants;
using SkillCraft.Contracts.Accounts;

namespace SkillCraft.Application.Accounts.Commands;

internal class VerifyPhoneCommandHandler : IRequestHandler<VerifyPhoneCommand, VerifyPhoneResult>
{
  private const string DefaultCountryCode = "CA";

  private readonly IMessageService _messageService;
  private readonly IOneTimePasswordService _oneTimePasswordService;
  private readonly ITokenService _tokenService;
  private readonly IUserService _userService;

  public VerifyPhoneCommandHandler(IMessageService messageService,
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
    // TODO(fpion): validate payload

    LocaleUnit locale = new(payload.Locale);
    User user = await FindUserAsync(payload.ProfileCompletionToken, cancellationToken);

    if (!string.IsNullOrWhiteSpace(payload.PhoneNumber))
    {
      return await HandlePhoneNumberAsync(payload.PhoneNumber.Trim(), locale, user, cancellationToken);
    }
    else if (payload.OneTimePassword != null)
    {
      return await HandleOneTimePasswordAsync(payload.OneTimePassword, user, cancellationToken);
    }

    throw new ArgumentException($"Exactly one of the following must be specified: {nameof(payload.PhoneNumber)}, {nameof(payload.OneTimePassword)}.", nameof(command));
  }

  private async Task<User> FindUserAsync(string profileCompletionToken, CancellationToken cancellationToken)
  {
    ValidatedToken validatedToken = await _tokenService.ValidateAsync(profileCompletionToken, consume: false, TokenTypes.Profile, cancellationToken);
    if (validatedToken.Subject == null)
    {
      throw new ArgumentException($"The '{nameof(validatedToken.Subject)}' claim is required.", nameof(profileCompletionToken));
    }
    Guid userId = validatedToken.GetUserId();
    return await _userService.FindAsync(userId, cancellationToken) ?? throw new ArgumentException($"The user 'Id={userId}' could not be found.", nameof(profileCompletionToken));
  }

  private async Task<VerifyPhoneResult> HandlePhoneNumberAsync(string phoneNumber, LocaleUnit locale, User user, CancellationToken cancellationToken)
  {
    Phone phone = new(DefaultCountryCode, phoneNumber, extension: null, e164Formatted: string.Empty);
    OneTimePassword oneTimePassword = await _oneTimePasswordService.CreateAsync(user, Purposes.ContactVerification, phone, cancellationToken);
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
    OneTimePasswordValidation oneTimePasswordValidation = new(oneTimePassword, sentMessage);
    return new VerifyPhoneResult(oneTimePasswordValidation);
  }

  private async Task<VerifyPhoneResult> HandleOneTimePasswordAsync(OneTimePasswordPayload payload, User user, CancellationToken cancellationToken)
  {
    OneTimePassword oneTimePassword = await _oneTimePasswordService.ValidateAsync(payload, Purposes.ContactVerification, cancellationToken);
    Guid userId = oneTimePassword.GetUserId();
    if (userId != user.Id)
    {
      throw new InvalidOneTimePasswordUserException(oneTimePassword, user);
    }
    Phone phone = oneTimePassword.GetPhone();
    phone.IsVerified = true;
    CreatedToken profileCompletion = await _tokenService.CreateAsync(user, phone, TokenTypes.Profile, cancellationToken);
    return new VerifyPhoneResult(profileCompletion.Token);
  }
}
