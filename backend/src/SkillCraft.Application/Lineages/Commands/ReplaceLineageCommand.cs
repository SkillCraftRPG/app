﻿using FluentValidation;
using MediatR;
using SkillCraft.Application.Languages.Queries;
using SkillCraft.Application.Lineages.Validators;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts.Lineages;
using SkillCraft.Domain;
using SkillCraft.Domain.Languages;
using SkillCraft.Domain.Lineages;

namespace SkillCraft.Application.Lineages.Commands;

public record ReplaceLineageCommand(Guid Id, ReplaceLineagePayload Payload, long? Version) : Activity, IRequest<LineageModel?>;

internal class ReplaceLineageCommandHandler : IRequestHandler<ReplaceLineageCommand, LineageModel?>
{
  private readonly ILineageQuerier _lineageQuerier;
  private readonly ILineageRepository _lineageRepository;
  private readonly IPermissionService _permissionService;
  private readonly ISender _sender;

  public ReplaceLineageCommandHandler(
    ILineageQuerier lineageQuerier,
    ILineageRepository lineageRepository,
    IPermissionService permissionService,
    ISender sender)
  {
    _lineageQuerier = lineageQuerier;
    _lineageRepository = lineageRepository;
    _permissionService = permissionService;
    _sender = sender;
  }

  public async Task<LineageModel?> Handle(ReplaceLineageCommand command, CancellationToken cancellationToken)
  {
    ReplaceLineagePayload payload = command.Payload;
    new ReplaceLineageValidator().ValidateAndThrow(payload);

    LineageId id = new(command.Id);
    Lineage? lineage = await _lineageRepository.LoadAsync(id, cancellationToken);
    if (lineage == null)
    {
      return null;
    }

    await _permissionService.EnsureCanUpdateAsync(command, lineage.GetMetadata(), cancellationToken);

    Lineage? reference = null;
    if (command.Version.HasValue)
    {
      reference = await _lineageRepository.LoadAsync(id, command.Version.Value, cancellationToken);
    }
    reference ??= lineage;

    Name name = new(payload.Name);
    if (name != reference.Name)
    {
      lineage.Name = name;
    }
    Description? description = Description.TryCreate(payload.Description);
    if (description != reference.Description)
    {
      lineage.Description = description;
    }

    Attributes attributes = new(payload.Attributes);
    if (attributes != reference.Attributes)
    {
      lineage.Attributes = attributes;
    }
    SetTraits(lineage, reference, payload);

    await SetLanguagesAsync(lineage, reference, payload.Languages, cancellationToken);
    SetNames(lineage, reference, payload.Names);

    Speeds speeds = new(payload.Speeds);
    if (speeds != reference.Speeds)
    {
      lineage.Speeds = speeds;
    }
    Size size = new(payload.Size.Category, new Roll(payload.Size.Roll));
    if (size != reference.Size)
    {
      lineage.Size = size;
    }
    Weight weight = new(
      new Roll(payload.Weight.Starved),
      new Roll(payload.Weight.Skinny),
      new Roll(payload.Weight.Normal),
      new Roll(payload.Weight.Overweight),
      new Roll(payload.Weight.Obese));
    if (weight != reference.Weight)
    {
      lineage.Weight = weight;
    }

    lineage.Update(command.GetUserId());
    await _sender.Send(new SaveLineageCommand(lineage), cancellationToken);

    return await _lineageQuerier.ReadAsync(lineage, cancellationToken);
  }

  private async Task SetLanguagesAsync(Lineage lineage, Lineage reference, LanguagesPayload payload, CancellationToken cancellationToken)
  {
    IReadOnlyCollection<Language> items = payload.Ids.Count == 0 ? []
      : await _sender.Send(new FindLanguagesQuery(payload.Ids), cancellationToken);
    Domain.Lineages.Languages languages = new(items, payload.Extra, payload.Text);
    if (languages != reference.Languages)
    {
      lineage.Languages = languages;
    }
  }

  private static void SetNames(Lineage lineage, Lineage reference, NamesModel payload)
  {
    Dictionary<string, IReadOnlyCollection<string>> custom = new(capacity: payload.Custom.Count);
    foreach (NameCategory category in payload.Custom)
    {
      custom[category.Key] = category.Values;
    }
    Names names = new(payload.Text, payload.Family, payload.Female, payload.Male, payload.Unisex, custom);
    if (names != reference.Names)
    {
      lineage.Names = names;
    }
  }

  private static void SetTraits(Lineage lineage, Lineage reference, ReplaceLineagePayload payload)
  {
    HashSet<Guid> traitIds = payload.Traits.Where(x => x.Id.HasValue).Select(x => x.Id!.Value).ToHashSet();
    foreach (Guid traitId in reference.Traits.Keys)
    {
      if (!traitIds.Contains(traitId))
      {
        lineage.RemoveTrait(traitId);
      }
    }

    foreach (TraitPayload traitPayload in payload.Traits)
    {
      Trait trait = new(new Name(traitPayload.Name), Description.TryCreate(traitPayload.Description));
      if (traitPayload.Id.HasValue)
      {
        if (!reference.Traits.TryGetValue(traitPayload.Id.Value, out Trait? existingTrait) || existingTrait != trait)
        {
          lineage.SetTrait(traitPayload.Id.Value, trait);
        }
      }
      else
      {
        lineage.AddTrait(trait);
      }
    }
  }
}
