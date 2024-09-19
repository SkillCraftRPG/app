using FluentValidation;
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

    lineage.Attributes = new AttributeBonuses(
      payload.Attributes.Agility ?? lineage.Attributes.Agility,
      payload.Attributes.Coordination ?? lineage.Attributes.Coordination,
      payload.Attributes.Intellect ?? lineage.Attributes.Intellect,
      payload.Attributes.Presence ?? lineage.Attributes.Presence,
      payload.Attributes.Sensitivity ?? lineage.Attributes.Sensitivity,
      payload.Attributes.Spirit ?? lineage.Attributes.Spirit,
      payload.Attributes.Vigor ?? lineage.Attributes.Vigor,
      payload.Attributes.Extra ?? lineage.Attributes.Extra);
    SetFeatures(lineage, payload);

    await SetLanguagesAsync(lineage, payload.Languages, cancellationToken);
    SetNames(lineage, payload.Names);

    lineage.Speeds = new Speeds(
      payload.Speeds.Walk ?? lineage.Speeds.Walk,
      payload.Speeds.Climb ?? lineage.Speeds.Climb,
      payload.Speeds.Swim ?? lineage.Speeds.Swim,
      payload.Speeds.Fly ?? lineage.Speeds.Fly,
      payload.Speeds.Hover ?? lineage.Speeds.Hover,
      payload.Speeds.Burrow ?? lineage.Speeds.Burrow);
    lineage.Size = new Size(
      payload.Size.Category ?? lineage.Size.Category,
      payload.Size.Roll == null ? lineage.Size.Roll : Roll.TryCreate(payload.Size.Roll.Value));
    lineage.Weight = new Weight(
      Roll.TryCreate(payload.Weight.Starved) ?? lineage.Weight.Starved,
      Roll.TryCreate(payload.Weight.Skinny) ?? lineage.Weight.Skinny,
      Roll.TryCreate(payload.Weight.Normal) ?? lineage.Weight.Normal,
      Roll.TryCreate(payload.Weight.Overweight) ?? lineage.Weight.Overweight,
      Roll.TryCreate(payload.Weight.Obese) ?? lineage.Weight.Obese);
    lineage.Ages = new Ages(
      payload.Ages.Adolescent == null ? lineage.Ages.Adolescent : payload.Ages.Adolescent.Value,
      payload.Ages.Adult == null ? lineage.Ages.Adult : payload.Ages.Adult.Value,
      payload.Ages.Mature == null ? lineage.Ages.Mature : payload.Ages.Mature.Value,
      payload.Ages.Venerable == null ? lineage.Ages.Venerable : payload.Ages.Venerable.Value);

    lineage.Update(command.GetUserId());
    await _sender.Send(new SaveLineageCommand(lineage), cancellationToken);

    return await _lineageQuerier.ReadAsync(lineage, cancellationToken);
  }

  private static void SetFeatures(Lineage lineage, UpdateLineagePayload payload)
  {
    foreach (UpdateFeaturePayload featurePayload in payload.Features)
    {
      if (featurePayload.Remove)
      {
        if (featurePayload.Id.HasValue)
        {
          lineage.RemoveFeature(featurePayload.Id.Value);
        }
      }
      else
      {
        Feature feature = new(new Name(featurePayload.Name), Description.TryCreate(featurePayload.Description));
        if (featurePayload.Id.HasValue)
        {
          lineage.SetFeature(featurePayload.Id.Value, feature);
        }
        else
        {
          lineage.AddFeature(feature);
        }
      }
    }
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
}
