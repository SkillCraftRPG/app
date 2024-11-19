namespace SkillCraft.Contracts.Lineages;

public class TraitModel
{
  public Guid Id { get; set; }

  public string Name { get; set; }
  public string? Description { get; set; }

  public TraitModel() : this(string.Empty)
  {
  }

  public TraitModel(string name)
  {
    Name = name;
  }

  public override bool Equals(object? obj) => obj is TraitModel trait && trait.Id == Id;
  public override int GetHashCode() => Id.GetHashCode();
  public override string ToString() => $"{Name} | {GetType()} (Id={Id})";
}
