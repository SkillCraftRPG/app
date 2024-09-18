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

public record UpdateLineageCommand(Guid Id, UpdateLineagePayload Payload) : Activity, IRequest<LineageModel?>;

internal class UpdateLineageCommandHandler : IRequestHandler<UpdateLineageCommand, LineageModel?>
{
  private readonly ILineageQuerier _lineageQuerier;
  private readonly ILineageRepository _lineageRepository;
  private readonly IPermissionService _permissionService;
  private readonly ISender _sender;

  public UpdateLineageCommandHandler(
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

  public async Task<LineageModel?> Handle(UpdateLineageCommand command, CancellationToken cancellationToken)
  {
    UpdateLineagePayload payload = command.Payload;
    new UpdateLineageValidator().ValidateAndThrow(payload);

    LineageId id = new(command.Id);
    Lineage? lineage = await _lineageRepository.LoadAsync(id, cancellationToken);
    if (lineage == null)
    {
      return null;
    }

    await _permissionService.EnsureCanUpdateAsync(command, lineage.GetMetadata(), cancellationToken);

    if (!string.IsNullOrWhiteSpace(payload.Name))
    {
      lineage.Name = new Name(payload.Name);
    }
    if (payload.Description != null)
    {
      lineage.Description = Description.TryCreate(payload.Description.Value);
    }

    lineage.Attributes = new Attributes(
      payload.Attributes.Agility ?? lineage.Attributes.Agility,
      payload.Attributes.Coordination ?? lineage.Attributes.Coordination,
      payload.Attributes.Intellect ?? lineage.Attributes.Intellect,
      payload.Attributes.Presence ?? lineage.Attributes.Presence,
      payload.Attributes.Sensitivity ?? lineage.Attributes.Sensitivity,
      payload.Attributes.Spirit ?? lineage.Attributes.Spirit,
      payload.Attributes.Vigor ?? lineage.Attributes.Vigor,
      payload.Attributes.Extra ?? lineage.Attributes.Extra);
    SetTraits(lineage, payload);

    await SetLanguagesAsync(lineage, payload.Languages, cancellationToken);
    SetNames(lineage, payload.Names);

    lineage.Speeds = new Speeds(
      payload.Speeds.Walk ?? lineage.Speeds.Walk,
      payload.Speeds.Climb ?? lineage.Speeds.Climb,
      payload.Speeds.Swim ?? lineage.Speeds.Swim,
      payload.Speeds.Fly ?? lineage.Speeds.Fly,
      payload.Speeds.Hover ?? lineage.Speeds.Hover,
      payload.Speeds.Burrow ?? lineage.Speeds.Burrow);

    lineage.Update(command.GetUserId());
    await _sender.Send(new SaveLineageCommand(lineage), cancellationToken);

    return await _lineageQuerier.ReadAsync(lineage, cancellationToken);
  }

  private async Task SetLanguagesAsync(Lineage lineage, UpdateLanguagesPayload payload, CancellationToken cancellationToken)
  {
    int extra = payload.Extra ?? lineage.Languages.Extra;
    string? text = payload.Text == null ? lineage.Languages.Text : payload.Text.Value;

    Domain.Lineages.Languages languages;
    if (payload.Ids == null)
    {
      languages = new(lineage.Languages.Ids, extra, text);
    }
    else
    {
      IReadOnlyCollection<Language> items = payload.Ids.Count == 0 ? []
        : await _sender.Send(new FindLanguagesQuery(payload.Ids), cancellationToken);
      languages = new(items, extra, text);
    }

    lineage.Languages = languages;
  }

  private static void SetNames(Lineage lineage, UpdateNamesPayload payload)
  {
    Dictionary<string, IReadOnlyCollection<string>> custom = new(lineage.Names.Custom);
    if (payload.Custom != null)
    {
      custom = new(capacity: payload.Custom.Count);
      foreach (NameCategory category in payload.Custom)
      {
        custom[category.Key] = category.Values;
      }
    }

    lineage.Names = new Names(
      payload.Text == null ? lineage.Names.Text : payload.Text.Value,
      payload.Family ?? lineage.Names.Family,
      payload.Female ?? lineage.Names.Female,
      payload.Male ?? lineage.Names.Male,
      payload.Unisex ?? lineage.Names.Unisex,
      custom);
  }

  private static void SetTraits(Lineage lineage, UpdateLineagePayload payload)
  {
    foreach (UpdateTraitPayload traitPayload in payload.Traits)
    {
      if (traitPayload.Remove)
      {
        if (traitPayload.Id.HasValue)
        {
          lineage.RemoveTrait(traitPayload.Id.Value);
        }
      }
      else
      {
        Trait trait = new(new Name(traitPayload.Name), Description.TryCreate(traitPayload.Description));
        if (traitPayload.Id.HasValue)
        {
          lineage.SetTrait(traitPayload.Id.Value, trait);
        }
        else
        {
          lineage.AddTrait(trait);
        }
      }
    }
  }
}