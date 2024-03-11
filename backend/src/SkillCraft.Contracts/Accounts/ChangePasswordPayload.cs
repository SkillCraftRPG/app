namespace SkillCraft.Contracts.Accounts;

public record ChangePasswordPayload
{
  public string Current { get; set; }
  public string New { get; set; }

  public ChangePasswordPayload() : this(string.Empty, string.Empty)
  {
  }

  public ChangePasswordPayload(string current, string @new)
  {
    Current = current;
    New = @new;
  }
}
