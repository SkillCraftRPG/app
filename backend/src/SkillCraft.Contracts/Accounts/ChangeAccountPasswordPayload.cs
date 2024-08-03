namespace SkillCraft.Contracts.Accounts;

public record ChangeAccountPasswordPayload
{
  public string Current { get; set; }
  public string New { get; set; }

  public ChangeAccountPasswordPayload() : this(string.Empty, string.Empty)
  {
  }

  public ChangeAccountPasswordPayload(string current, string @new)
  {
    Current = current;
    New = @new;
  }
}
