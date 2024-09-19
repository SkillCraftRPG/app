namespace SkillCraft.Contracts.Lineages;

public class FeatureModel
{
  public Guid Id { get; set; }

  public string Name { get; set; }
  public string? Description { get; set; }

  public FeatureModel() : this(string.Empty)
  {
  }

  public FeatureModel(string name)
  {
    Name = name;
  }

  public override bool Equals(object? obj) => obj is FeatureModel feature && feature.Id == Id;
  public override int GetHashCode() => Id.GetHashCode();
  public override string ToString() => $"{Name} | {GetType()} (Id={Id})";
}
