namespace SkillCraft.Contracts.Accounts;

public record OneTimePasswordValidation
{
  public Guid Id { get; set; }
  public SentMessage SentMessage { get; set; }

  public OneTimePasswordValidation() : this(default, new())
  {
  }

  public OneTimePasswordValidation(Guid id, SentMessage sentMessage)
  {
    Id = id;
    SentMessage = sentMessage;
  }
}
