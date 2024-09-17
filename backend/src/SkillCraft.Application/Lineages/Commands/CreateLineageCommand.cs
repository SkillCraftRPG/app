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

    Lineage? parent = null;
    if (payload.ParentId.HasValue)
    {
      LineageId parentId = new(payload.ParentId.Value);
      parent = await _lineageRepository.LoadAsync(parentId, cancellationToken)
        ?? throw new AggregateNotFoundException<Lineage>(parentId.AggregateId, nameof(payload.ParentId));
    }

    UserId userId = command.GetUserId();
    Lineage lineage = new(command.GetWorldId(), parent, new Name(payload.Name), userId)
    {
      Description = Description.TryCreate(payload.Description),
      Attributes = new(payload.Attributes)
    };
    SetTraits(lineage, payload);

    await SetLanguagesAsync(lineage, payload.Languages, cancellationToken);
    SetNames(lineage, payload.Names);

    lineage.Update(userId);
    await _sender.Send(new SaveLineageCommand(lineage), cancellationToken);

    return await _lineageQuerier.ReadAsync(lineage, cancellationToken);
  }

  private async Task SetLanguagesAsync(Lineage lineage, LanguagesPayload payload, CancellationToken cancellationToken)
  {
    IReadOnlyCollection<Language> languages = payload.Ids.Count == 0 ? []
      : await _sender.Send(new FindLanguagesQuery(payload.Ids), cancellationToken);
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

  private static void SetTraits(Lineage lineage, CreateLineagePayload payload)
  {
    foreach (TraitPayload traitPayload in payload.Traits)
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
