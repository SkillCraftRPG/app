namespace SkillCraft.Contracts.Castes;

public record UpdateTraitPayload
{
  public Guid? Id { get; set; }

  public string Name { get; set; }
  public string? Description { get; set; }

  public bool Remove { get; set; }

  public UpdateTraitPayload() : this(string.Empty)
  {
  }

  public UpdateTraitPayload(string name)
  {
    Name = name;
  }
}
