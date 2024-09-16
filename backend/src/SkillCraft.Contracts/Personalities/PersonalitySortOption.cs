using Logitar.Portal.Contracts.Search;

namespace SkillCraft.Contracts.Personalities;

public record PersonalitySortOption : SortOption
{
  public new PersonalitySort Field
  {
    get => Enum.Parse<PersonalitySort>(base.Field);
    set => base.Field = value.ToString();
  }

  public PersonalitySortOption(PersonalitySort field, bool isDescending = false) : base(field.ToString(), isDescending)
  {
  }
}
