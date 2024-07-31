using FluentValidation;
using Logitar.Identity.Domain.Shared;
using Logitar.Portal.Contracts.Messages;
using Logitar.Portal.Contracts.Passwords;
using Logitar.Portal.Contracts.Users;
using MediatR;
using SkillCraft.Application.Accounts.Constants;
using SkillCraft.Application.Accounts.Validators;
using SkillCraft.Contracts.Accounts;

namespace SkillCraft.Application.Accounts.Commands;

internal class ChangePhoneCommandHandler : IRequestHandler<ChangePhoneCommand, ChangePhoneResult>
{
  private readonly IMessageService _messageService;
  private readonly IOneTimePasswordService _oneTimePasswordService;
  private readonly IUserService _userService;

  public ChangePhoneCommandHandler(IMessageService messageService, IOneTimePasswordService oneTimePasswordService, IUserService userService)
  {
    _messageService = messageService;
    _oneTimePasswordService = oneTimePasswordService;
    _userService = userService;
  }

  public async Task<ChangePhoneResult> Handle(ChangePhoneCommand command, CancellationToken cancellationToken)
  {
    ChangePhonePayload payload = command.Payload;
    new ChangePhoneValidator().ValidateAndThrow(payload);

    LocaleUnit locale = new(payload.Locale);
    User user = command.GetUser();

    if (payload.Phone != null)
    {
      return await HandlePhoneAsync(payload.Phone, locale, user, cancellationToken);
    }
    else if (payload.OneTimePassword != null)
    {
      return await HandleOneTimePasswordAsync(payload.OneTimePassword, user, cancellationToken);
    }

    throw new ArgumentException($"Exactly one of the following must be specified: {nameof(payload.Phone)}, {nameof(payload.OneTimePassword)}.", nameof(command));
  }

  private async Task<ChangePhoneResult> HandlePhoneAsync(AccountPhone accountPhone, LocaleUnit locale, User user, CancellationToken cancellationToken)
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
    return new ChangePhoneResult(validation);
  }

  private async Task<ChangePhoneResult> HandleOneTimePasswordAsync(OneTimePasswordPayload payload, User user, CancellationToken cancellationToken)
  {
    OneTimePassword oneTimePassword = await _oneTimePasswordService.ValidateAsync(payload, Purposes.ContactVerification, cancellationToken);
    if (oneTimePassword.GetUserId() != user.Id)
    {
      throw new InvalidOneTimePasswordUserException(oneTimePassword, user);
    }
    PhonePayload phone = oneTimePassword.GetPhonePayload(isVerified: true);
    if (user.Phone == null || !user.Phone.IsEqualTo(phone))
    {
      user = await _userService.UpdateAsync(user, phone, cancellationToken);
    }
    return new ChangePhoneResult(user.ToUserProfile());
  }
}
