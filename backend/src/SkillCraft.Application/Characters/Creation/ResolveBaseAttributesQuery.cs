using MediatR;
using SkillCraft.Contracts.Characters;
using SkillCraft.Domain.Aspects;
using SkillCraft.Domain.Characters;
using SkillCraft.Domain.Lineages;
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

    if (!mandatoryAttributes.Remove(payload.Best))
    {
      throw new InvalidAspectAttributeSelectionException(payload.Best, nameof(payload.Best));
    }
    if (!mandatoryAttributes.Remove(payload.Worst))
    {
      throw new InvalidAspectAttributeSelectionException(payload.Worst, nameof(payload.Worst));
    }
    int index = 0;
    foreach (Attribute optionalAttribute in payload.Optional)
    {
      if (!optionalAttributes.Remove(optionalAttribute))
      {
        throw new InvalidAspectAttributeSelectionException(optionalAttribute, $"{nameof(payload.Optional)}[{index}]");
      }
      index++;
    }

    int extra = query.Lineage.Attributes.Extra + (query.Parent?.Attributes.Extra ?? 0);
    if (payload.Extra.Distinct().Count() != extra)
    {
      throw new InvalidExtraAttributesException(payload.Extra, extra, nameof(payload.Extra));
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
