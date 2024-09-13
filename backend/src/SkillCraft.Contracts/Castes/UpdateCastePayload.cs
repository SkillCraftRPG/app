﻿namespace SkillCraft.Contracts.Castes;

public record UpdateCastePayload
{
  public string? Name { get; set; }
  public Change<string>? Description { get; set; }

  public Change<Skill>? Skill { get; set; }
  public Change<string>? WealthRoll { get; set; }

  public List<TraitModel> Traits { get; set; } // TODO(fpion): how to remove a trait?

  public UpdateCastePayload() : this(string.Empty)
  {
  }

  public UpdateCastePayload(string name)
  {
    Name = name;

    Traits = [];
  }

  public override string ToString() => $"{Name} | {base.ToString()}";
}