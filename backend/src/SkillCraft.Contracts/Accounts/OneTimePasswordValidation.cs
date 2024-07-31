using Logitar.Portal.Contracts.Passwords;

namespace SkillCraft.Contracts.Accounts;

public record OneTimePasswordValidation
{
  public Guid Id { get; set; }
  public SentMessage SentMessage { get; set; }

  public OneTimePasswordValidation() : this(Guid.Empty, new())
  {
  }

  public OneTimePasswordValidation(OneTimePassword oneTimePassword, SentMessage sentMessage) : this(oneTimePassword.Id, sentMessage)
  {
  }

  public OneTimePasswordValidation(Guid id, SentMessage sentMessage)
  {
    Id = id;
    SentMessage = sentMessage;
  }
}
