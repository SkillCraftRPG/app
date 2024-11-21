using Logitar.Portal.Contracts.Search;

namespace SkillCraft.Contracts.Characters;

public record CharacterSortOption : SortOption
{
  public new CharacterSort Field
  {
    get => Enum.Parse<CharacterSort>(base.Field);
    set => base.Field = value.ToString();
  }

  public CharacterSortOption(CharacterSort field, bool isDescending = false) : base(field.ToString(), isDescending)
  {
  }
}
