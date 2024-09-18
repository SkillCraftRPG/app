﻿namespace SkillCraft.Contracts.Lineages;

public record UpdateNamesPayload
{
  public Change<string>? Text { get; set; }
  public List<string>? Family { get; set; }
  public List<string>? Female { get; set; }
  public List<string>? Male { get; set; }
  public List<string>? Unisex { get; set; }
  public List<NameCategory>? Custom { get; set; }
}