using FluentValidation;
using SkillCraft.Domain.Validators;

namespace SkillCraft.Domain;

public record EntityKey
{
  public EntityType Type { get; }
  public Guid Id { get; }

  public EntityKey(EntityType type, Guid id)
  {
    Type = type;
    Id = id;
    new EntityKeyValidator().ValidateAndThrow(this);
  }
}
