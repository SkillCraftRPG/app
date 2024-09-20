using Logitar.Portal.Contracts.Search;

namespace SkillCraft.Contracts.Talents;

public record TalentSortOption : SortOption
{
  public new TalentSort Field
  {
    get => Enum.Parse<TalentSort>(base.Field);
    set => base.Field = value.ToString();
  }

  public TalentSortOption(TalentSort field, bool isDescending = false) : base(field.ToString(), isDescending)
  {
  }
}
