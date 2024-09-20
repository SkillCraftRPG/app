using FluentValidation;
using MediatR;
using SkillCraft.Application.Languages.Queries;
using SkillCraft.Application.Lineages.Validators;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts.Lineages;
using SkillCraft.Domain;
using SkillCraft.Domain.Languages;
using SkillCraft.Domain.Lineages;
using SkillCraft.Domain.Worlds;
using Action = SkillCraft.Application.Permissions.Action;

namespace SkillCraft.Application.Lineages.Commands;

public record CreateLineageCommand(CreateLineagePayload Payload) : Activity, IRequest<LineageModel>;

internal class CreateLineageCommandHandler : IRequestHandler<CreateLineageCommand, LineageModel>
{
  private readonly ILineageQuerier _lineageQuerier;
  private readonly ILineageRepository _lineageRepository;
  private readonly IPermissionService _permissionService;
  private readonly ISender _sender;

  public CreateLineageCommandHandler(
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

  public async Task<LineageModel> Handle(CreateLineageCommand command, CancellationToken cancellationToken)
  {
    CreateLineagePayload payload = command.Payload;
    new CreateLineageValidator().ValidateAndThrow(payload);

    await _permissionService.EnsureCanCreateAsync(command, EntityType.Lineage, cancellationToken);

    WorldId worldId = command.GetWorldId();

    Lineage? parent = null;
    if (payload.ParentId.HasValue)
    {
      LineageId parentId = new(payload.ParentId.Value);
      parent = await _lineageRepository.LoadAsync(parentId, cancellationToken)
        ?? throw new AggregateNotFoundException<Lineage>(parentId.AggregateId, nameof(payload.ParentId));
      if (parent.WorldId != worldId)
      {
        throw new PermissionDeniedException(Action.Preview, EntityType.Lineage, command.GetUser(), command.GetWorld(), parent.Id.ToGuid());
      }
      else if (parent.ParentId.HasValue)
      {
        throw new InvalidParentLineageException(parent, nameof(payload.ParentId));
      }
    }

    UserId userId = command.GetUserId();
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
    SetFeatures(lineage, payload);

    await SetLanguagesAsync(command, lineage, payload.Languages, cancellationToken);
    SetNames(lineage, payload.Names);

    lineage.Update(userId);
    await _sender.Send(new SaveLineageCommand(lineage), cancellationToken);

    return await _lineageQuerier.ReadAsync(lineage, cancellationToken);
  }

  private static void SetFeatures(Lineage lineage, CreateLineagePayload payload)
  {
    foreach (FeaturePayload featurePayload in payload.Features)
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

  private async Task SetLanguagesAsync(Activity activity, Lineage lineage, LanguagesPayload payload, CancellationToken cancellationToken)
  {
    IReadOnlyCollection<Language> languages = payload.Ids.Count == 0 ? []
      : await _sender.Send(new FindLanguagesQuery(activity, payload.Ids), cancellationToken);
    lineage.Languages = new Domain.Lineages.Languages(languages, payload.Extra, payload.Text);
  }

  private static void SetNames(Lineage lineage, NamesModel payload)
  {
    Dictionary<string, IReadOnlyCollection<string>> custom = new(capacity: payload.Custom.Count);
    foreach (NameCategory category in payload.Custom)
    {
      custom[category.Key] = category.Values;
    }
    lineage.Names = new Names(payload.Text, payload.Family, payload.Female, payload.Male, payload.Unisex, custom);
  }
}
