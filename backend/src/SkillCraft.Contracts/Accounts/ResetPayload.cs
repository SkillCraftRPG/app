namespace SkillCraft.Contracts.Accounts;

public record ResetPayload
{
  public string Token { get; set; }
  public string Password { get; set; }

  public ResetPayload() : this(string.Empty, string.Empty)
  {
  }

  public ResetPayload(string token, string password)
  {
    Token = token;
    Password = password;
  }
}
