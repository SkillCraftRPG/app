namespace SkillCraft.Contracts.Parties;

public record ReplacePartyPayload
{
  public string Name { get; set; }
  public string? Description { get; set; }

  public ReplacePartyPayload() : this(string.Empty)
  {
  }

  public ReplacePartyPayload(string name)
  {
    Name = name;
  }
}
