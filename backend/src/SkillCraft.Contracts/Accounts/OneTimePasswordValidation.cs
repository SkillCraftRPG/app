using Logitar.Portal.Contracts.Passwords;

namespace SkillCraft.Contracts.Accounts;

public record OneTimePasswordValidation
{
  public Guid OneTimePasswordId { get; }
  public SentMessage SentMessage { get; }

  public OneTimePasswordValidation(OneTimePassword oneTimePassword, SentMessage sentMessage)
  {
    OneTimePasswordId = oneTimePassword.Id;
    SentMessage = sentMessage;
  }
}
