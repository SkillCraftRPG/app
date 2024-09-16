﻿namespace SkillCraft.Contracts.Personalities;

public record UpdatePersonalityPayload
{
  public string? Name { get; set; }
  public Change<string>? Description { get; set; }

  public Change<Attribute?>? Attribute { get; set; }
  public Change<Guid?>? GiftId { get; set; }
}
