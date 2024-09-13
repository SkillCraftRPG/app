﻿using Logitar.Portal.Contracts.Search;

namespace SkillCraft.Contracts.Castes;

public record CasteSortOption : SortOption
{
  public new CasteSort Field
  {
    get => Enum.Parse<CasteSort>(base.Field);
    set => base.Field = value.ToString();
  }

  public CasteSortOption(CasteSort field, bool isDescending = false) : base(field.ToString(), isDescending)
  {
  }
}