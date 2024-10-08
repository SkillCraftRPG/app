﻿namespace SkillCraft.Contracts.Lineages;

public record ReplaceLineagePayload
{
  public string Name { get; set; }
  public string? Description { get; set; }

  public AttributeBonusesModel Attributes { get; set; }
  public List<FeaturePayload> Features { get; set; }

  public LanguagesPayload Languages { get; set; }
  public NamesModel Names { get; set; }

  public SpeedsModel Speeds { get; set; }
  public SizeModel Size { get; set; }
  public WeightModel Weight { get; set; }
  public AgesModel Ages { get; set; }

  public ReplaceLineagePayload() : this(string.Empty)
  {
  }

  public ReplaceLineagePayload(string name)
  {
    Name = name;

    Attributes = new();
    Features = [];

    Languages = new();
    Names = new();

    Speeds = new();
    Size = new();
    Weight = new();
    Ages = new();
  }
}
