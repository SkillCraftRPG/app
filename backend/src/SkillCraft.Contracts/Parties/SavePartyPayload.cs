namespace SkillCraft.Contracts.Parties;

public record SavePartyPayload
{
  public string Name { get; set; }
  public string? Description { get; set; }

  public SavePartyPayload() : this(string.Empty)
  {
  }

  public SavePartyPayload(string name)
  {
    Name = name;
  }
}
