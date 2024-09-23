namespace SkillCraft.Contracts.Parties;

public record CreatePartyPayload
{
  public string Name { get; set; }
  public string? Description { get; set; }

  public CreatePartyPayload() : this(string.Empty)
  {
  }

  public CreatePartyPayload(string name)
  {
    Name = name;
  }
}
