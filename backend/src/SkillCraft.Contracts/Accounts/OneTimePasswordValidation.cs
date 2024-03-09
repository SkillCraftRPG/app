using Logitar.Portal.Contracts.Passwords;

namespace SkillCraft.Contracts.Accounts;

public record OneTimePasswordValidation
{
  public Guid OneTimePasswordId { get; private set; }
  public SentMessage SentMessage { get; private set; }

  public OneTimePasswordValidation(OneTimePassword oneTimePassword, SentMessage sentMessage)
  {
    OneTimePasswordId = oneTimePassword.Id;
    SentMessage = sentMessage;
  }
}
