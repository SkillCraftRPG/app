namespace SkillCraft.Contracts.Accounts;

public record CredentialsPayload
{
  public string EmailAddress { get; set; }
  public string? Password { get; set; }

  public CredentialsPayload() : this(string.Empty)
  {
  }

  public CredentialsPayload(string emailAddress, string? password = null)
  {
    EmailAddress = emailAddress;
    Password = password;
  }
}
