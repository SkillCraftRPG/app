﻿using Logitar.Portal.Contracts;
using SkillCraft.Contracts.Aspects;
using SkillCraft.Contracts.Castes;
using SkillCraft.Contracts.Customizations;
using SkillCraft.Contracts.Educations;
using SkillCraft.Contracts.Lineages;
using SkillCraft.Contracts.Personalities;
using SkillCraft.Contracts.Worlds;

namespace SkillCraft.Contracts.Characters;

public class CharacterModel : Aggregate
{
  public WorldModel World { get; set; } = new();

  public string Name { get; set; } = string.Empty;
  public string? PlayerName { get; set; }

  public LineageModel Lineage { get; set; } = new();
  public double Height { get; set; }
  public double Weight { get; set; }
  public int Age { get; set; }
  public List<CharacterLanguageModel> Languages { get; set; } = [];

  public PersonalityModel Personality { get; set; } = new();
  public List<CustomizationModel> Customizations { get; set; } = [];

  public List<AspectModel> Aspects { get; set; } = [];

  public BaseAttributesModel BaseAttributes { get; set; } = new();

  public CasteModel Caste { get; set; } = new();
  public EducationModel Education { get; set; } = new();

  public List<CharacterTalentModel> Talents { get; set; } = [];

  public List<InventoryModel> Inventory { get; set; } = [];

  public override string ToString() => $"{Name} | {base.ToString()}";
}
