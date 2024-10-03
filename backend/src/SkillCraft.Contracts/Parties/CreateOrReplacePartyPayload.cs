namespace SkillCraft.Contracts.Parties;

public record CreateOrReplacePartyPayload
{
  public string Name { get; set; }
  public string? Description { get; set; }

  public CreateOrReplacePartyPayload() : this(string.Empty)
  {
  }

  public CreateOrReplacePartyPayload(string name)
  {
    Name = name;
  }
}
