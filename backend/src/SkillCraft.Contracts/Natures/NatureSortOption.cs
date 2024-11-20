using Logitar.Portal.Contracts.Search;

namespace SkillCraft.Contracts.Natures;

public record NatureSortOption : SortOption
{
  public new NatureSort Field
  {
    get => Enum.Parse<NatureSort>(base.Field);
    set => base.Field = value.ToString();
  }

  public NatureSortOption(NatureSort field, bool isDescending = false) : base(field.ToString(), isDescending)
  {
  }
}
