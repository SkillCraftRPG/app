using FluentValidation;
using MediatR;
using SkillCraft.Application.Languages.Queries;
using SkillCraft.Application.Lineages.Validators;
using SkillCraft.Application.Permissions;
using SkillCraft.Application.Storages;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Lineages;
using SkillCraft.Domain;
using SkillCraft.Domain.Languages;
using SkillCraft.Domain.Lineages;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Application.Lineages.Commands;

public record CreateOrReplaceLineageResult(LineageModel? Lineage = null, bool Created = false);

/// <exception cref="NotEnoughAvailableStorageException"></exception>
/// <exception cref="PermissionDeniedException"></exception>
/// <exception cref="ValidationException"></exception>
public record CreateOrReplaceLineageCommand(Guid? Id, CreateOrReplaceLineagePayload Payload, long? Version) : Activity, IRequest<CreateOrReplaceLineageResult>;

internal class CreateOrReplaceLineageCommandHandler : IRequestHandler<CreateOrReplaceLineageCommand, CreateOrReplaceLineageResult>
{
  private readonly ILineageQuerier _lineageQuerier;
  private readonly ILineageRepository _lineageRepository;
  private readonly IPermissionService _permissionService;
  private readonly ISender _sender;

  public CreateOrReplaceLineageCommandHandler(
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

  public async Task<CreateOrReplaceLineageResult> Handle(CreateOrReplaceLineageCommand command, CancellationToken cancellationToken)
  {
    new CreateOrReplaceLineageValidator().ValidateAndThrow(command.Payload);

    Lineage? lineage = await FindAsync(command, cancellationToken);
    bool created = false;
    if (lineage == null)
    {
      if (command.Version.HasValue)
      {
        return new CreateOrReplaceLineageResult();
      }

      lineage = await CreateAsync(command, cancellationToken);
      created = true;
    }
    else
    {
      await ReplaceAsync(command, lineage, cancellationToken);
    }

    await _sender.Send(new SaveLineageCommand(lineage), cancellationToken);

    LineageModel model = await _lineageQuerier.ReadAsync(lineage, cancellationToken);
    return new CreateOrReplaceLineageResult(model, created);
  }

  private async Task<Lineage?> FindAsync(CreateOrReplaceLineageCommand command, CancellationToken cancellationToken)
  {
    if (!command.Id.HasValue)
    {
      return null;
    }

    LineageId id = new(command.GetWorldId(), command.Id.Value);
    return await _lineageRepository.LoadAsync(id, cancellationToken);
  }

  private async Task<Lineage> CreateAsync(CreateOrReplaceLineageCommand command, CancellationToken cancellationToken)
  {
    await _permissionService.EnsureCanCreateAsync(command, EntityType.Lineage, cancellationToken);

    CreateOrReplaceLineagePayload payload = command.Payload;
    UserId userId = command.GetUserId();
    WorldId worldId = command.GetWorldId();

    Lineage? parent = null;
    if (payload.ParentId.HasValue)
    {
      LineageId parentId = new(worldId, payload.ParentId.Value);
      parent = await _lineageRepository.LoadAsync(parentId, cancellationToken)
        ?? throw new LineageNotFoundException(parentId, nameof(payload.ParentId));
      if (parent.ParentId.HasValue)
      {
        throw new InvalidParentLineageException(parent, nameof(payload.ParentId));
      }
    }

    Lineage lineage = new(worldId, parent, new Name(payload.Name), userId)
    {
      Description = Description.TryCreate(payload.Description),
      Attributes = new AttributeBonuses(payload.Attributes),
      Speeds = new Speeds(payload.Speeds),
      Size = new Size(payload.Size.Category, Roll.TryCreate(payload.Size.Roll)),
      Weight = new Weight(
        Roll.TryCreate(payload.Weight.Starved),
        Roll.TryCreate(payload.Weight.Skinny),
        Roll.TryCreate(payload.Weight.Normal),
        Roll.TryCreate(payload.Weight.Overweight),
        Roll.TryCreate(payload.Weight.Obese)),
      Ages = new Ages(payload.Ages)
    };
    SetFeatures(lineage, lineage, payload);

    await SetLanguagesAsync(command, lineage, lineage, payload.Languages, cancellationToken);
    SetNames(lineage, lineage, payload.Names);

    lineage.Update(userId);

    return lineage;
  }

  private async Task ReplaceAsync(CreateOrReplaceLineageCommand command, Lineage lineage, CancellationToken cancellationToken)
  {
    await _permissionService.EnsureCanUpdateAsync(command, lineage.GetMetadata(), cancellationToken);

    CreateOrReplaceLineagePayload payload = command.Payload;
    UserId userId = command.GetUserId();

    Lineage? reference = null;
    if (command.Version.HasValue)
    {
      reference = await _lineageRepository.LoadAsync(lineage.Id, command.Version.Value, cancellationToken);
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

    AttributeBonuses attributes = new(payload.Attributes);
    if (attributes != reference.Attributes)
    {
      lineage.Attributes = attributes;
    }
    SetFeatures(lineage, reference, payload);

    await SetLanguagesAsync(command, lineage, reference, payload.Languages, cancellationToken);
    SetNames(lineage, reference, payload.Names);

    Speeds speeds = new(payload.Speeds);
    if (speeds != reference.Speeds)
    {
      lineage.Speeds = speeds;
    }
    Size size = new(payload.Size.Category, Roll.TryCreate(payload.Size.Roll));
    if (size != reference.Size)
    {
      lineage.Size = size;
    }
    Weight weight = new(
      Roll.TryCreate(payload.Weight.Starved),
      Roll.TryCreate(payload.Weight.Skinny),
      Roll.TryCreate(payload.Weight.Normal),
      Roll.TryCreate(payload.Weight.Overweight),
      Roll.TryCreate(payload.Weight.Obese));
    if (weight != reference.Weight)
    {
      lineage.Weight = weight;
    }
    Ages ages = new(payload.Ages);
    if (ages != reference.Ages)
    {
      lineage.Ages = ages;
    }

    lineage.Update(userId);
  }

  private async Task SetLanguagesAsync(Activity activity, Lineage lineage, Lineage reference, LanguagesPayload payload, CancellationToken cancellationToken)
  {
    IReadOnlyCollection<Language> items = payload.Ids.Count == 0
      ? []
      : await _sender.Send(new FindLanguagesQuery(activity, payload.Ids), cancellationToken);
    Domain.Lineages.Languages languages = new(items, payload.Extra, payload.Text);
    if (languages != reference.Languages)
    {
      lineage.Languages = languages;
    }
  }

  private static void SetFeatures(Lineage lineage, Lineage reference, CreateOrReplaceLineagePayload payload)
  {
    HashSet<Guid> featureIds = payload.Features.Where(x => x.Id.HasValue).Select(x => x.Id!.Value).ToHashSet();
    foreach (Guid featureId in reference.Features.Keys)
    {
      if (!featureIds.Contains(featureId))
      {
        lineage.RemoveFeature(featureId);
      }
    }

    foreach (FeaturePayload featurePayload in payload.Features)
    {
      Feature feature = new(new Name(featurePayload.Name), Description.TryCreate(featurePayload.Description));
      if (featurePayload.Id.HasValue)
      {
        if (!reference.Features.TryGetValue(featurePayload.Id.Value, out Feature? existingFeature) || existingFeature != feature)
        {
          lineage.SetFeature(featurePayload.Id.Value, feature);
        }
      }
      else
      {
        lineage.AddFeature(feature);
      }
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
}
