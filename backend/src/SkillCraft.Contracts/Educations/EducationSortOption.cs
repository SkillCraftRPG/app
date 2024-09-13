using Logitar.Portal.Contracts.Search;

namespace SkillCraft.Contracts.Educations;

public record EducationSortOption : SortOption
{
  public new EducationSort Field
  {
    get => Enum.Parse<EducationSort>(base.Field);
    set => base.Field = value.ToString();
  }

  public EducationSortOption(EducationSort field, bool isDescending = false) : base(field.ToString(), isDescending)
  {
  }
}
