using MediatR;
using SkillCraft.Contracts.Characters;
using SkillCraft.Domain.Aspects;
using SkillCraft.Domain.Characters;
using SkillCraft.Domain.Lineages;
using SkillCraft.Domain.Worlds;
using Attribute = SkillCraft.Contracts.Attribute;

namespace SkillCraft.Application.Characters.Creation;

internal record ResolveBaseAttributesQuery(BaseAttributesPayload Payload, IReadOnlyCollection<Aspect> Aspects, Lineage Lineage, Lineage? Parent)
  : IRequest<BaseAttributes>;

internal class ResolveBaseAttributesQueryHandler : IRequestHandler<ResolveBaseAttributesQuery, BaseAttributes>
{
  public Task<BaseAttributes> Handle(ResolveBaseAttributesQuery query, CancellationToken cancellationToken)
  {
    BaseAttributesPayload payload = query.Payload;
    IReadOnlyCollection<Aspect> aspects = query.Aspects;

    List<Attribute> mandatoryAttributes = new(capacity: aspects.Count * 2);
    List<Attribute> optionalAttributes = new(capacity: aspects.Count * 2);
    foreach (Aspect aspect in aspects)
    {
      if (aspect.Attributes.Mandatory1.HasValue)
      {
        mandatoryAttributes.Add(aspect.Attributes.Mandatory1.Value);
      }
      if (aspect.Attributes.Mandatory2.HasValue)
      {
        mandatoryAttributes.Add(aspect.Attributes.Mandatory2.Value);
      }
      if (aspect.Attributes.Optional1.HasValue)
      {
        optionalAttributes.Add(aspect.Attributes.Optional1.Value);
      }
      if (aspect.Attributes.Optional2.HasValue)
      {
        optionalAttributes.Add(aspect.Attributes.Optional2.Value);
      }
    }

    WorldId worldId = aspects.Select(a => a.WorldId).Distinct().Single();
    IEnumerable<Guid> aspectIds = aspects.Select(a => a.EntityId).Distinct();

    if (!mandatoryAttributes.Remove(payload.Best))
    {
      throw new InvalidAspectAttributeSelectionException(worldId, aspectIds, payload.Best, nameof(payload.Best));
    }
    if (!mandatoryAttributes.Remove(payload.Worst))
    {
      throw new InvalidAspectAttributeSelectionException(worldId, aspectIds, payload.Worst, nameof(payload.Worst));
    }
    int index = 0;
    foreach (Attribute optionalAttribute in payload.Optional)
    {
      if (!optionalAttributes.Remove(optionalAttribute))
      {
        throw new InvalidAspectAttributeSelectionException(worldId, aspectIds, optionalAttribute, $"{nameof(payload.Optional)}[{index}]");
      }
      index++;
    }

    Lineage lineage = query.Lineage;
    int extra = lineage.Attributes.Extra + (query.Parent?.Attributes.Extra ?? 0);
    if (payload.Extra.Distinct().Count() != extra)
    {
      throw new InvalidExtraAttributesException(lineage, payload.Extra, extra, nameof(payload.Extra));
    }

    BaseAttributes baseAttributes = new(
      payload.Agility,
      payload.Coordination,
      payload.Intellect,
      payload.Presence,
      payload.Sensitivity,
      payload.Spirit,
      payload.Vigor,
      payload.Best,
      payload.Worst,
      mandatoryAttributes,
      payload.Optional,
      payload.Extra);
    return Task.FromResult(baseAttributes);
  }
}
