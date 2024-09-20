using FluentValidation;
using Logitar.Portal.Contracts.Search;
using MediatR;
using SkillCraft.Application.Characters.Validators;
using SkillCraft.Application.Lineages;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts.Characters;
using SkillCraft.Contracts.Lineages;
using SkillCraft.Domain;
using SkillCraft.Domain.Characters;
using SkillCraft.Domain.Lineages;

namespace SkillCraft.Application.Characters.Commands;

public record CreateCharacterCommand(CreateCharacterPayload Payload) : Activity, IRequest<CharacterModel>;

internal class CreateCharacterCommandHandler : IRequestHandler<CreateCharacterCommand, CharacterModel>
{
  private readonly ICharacterQuerier _characterQuerier;
  private readonly ILineageQuerier _lineageQuerier;
  private readonly ILineageRepository _lineageRepository;
  private readonly IPermissionService _permissionService;
  private readonly ISender _sender;

  public CreateCharacterCommandHandler(
    ICharacterQuerier characterQuerier,
    ILineageQuerier lineageQuerier,
    ILineageRepository lineageRepository,
    IPermissionService permissionService,
    ISender sender)
  {
    _characterQuerier = characterQuerier;
    _lineageQuerier = lineageQuerier;
    _lineageRepository = lineageRepository;
    _permissionService = permissionService;
    _sender = sender;
  }

  public async Task<CharacterModel> Handle(CreateCharacterCommand command, CancellationToken cancellationToken)
  {
    CreateCharacterPayload payload = command.Payload;
    new CreateCharacterValidator().ValidateAndThrow(payload);

    await _permissionService.EnsureCanCreateAsync(command, EntityType.Character, cancellationToken);

    Lineage lineage = await ResolveLineageAsync(command, payload.LineageId, cancellationToken);

    Character character = new(
      command.GetWorldId(),
      new Name(payload.Name),
      PlayerName.TryCreate(payload.Player),
      lineage,
      payload.Height,
      payload.Weight,
      payload.Age,
      command.GetUserId());

    await _sender.Send(new SaveCharacterCommand(character), cancellationToken);

    return await _characterQuerier.ReadAsync(character, cancellationToken);
  }

  private async Task<Lineage> ResolveLineageAsync(Activity activity, Guid id, CancellationToken cancellationToken)
  {
    LineageId lineageId = new(id);
    Lineage lineage = await _lineageRepository.LoadAsync(lineageId, cancellationToken)
      ?? throw new AggregateNotFoundException<Lineage>(lineageId.AggregateId, nameof(CreateCharacterPayload.LineageId));

    await _permissionService.EnsureCanPreviewAsync(activity, lineage.GetMetadata(), cancellationToken);

    if (lineage.ParentId == null)
    {
      SearchLineagesPayload payload = new()
      {
        ParentId = lineage.Id.ToGuid(),
        Limit = 1
      };
      SearchResults<LineageModel> results = await _lineageQuerier.SearchAsync(activity.GetWorldId(), payload, cancellationToken);
      if (results.Total > 0)
      {
        throw new InvalidCharacterLineageException(lineage, nameof(CreateCharacterPayload.LineageId));
      }
    }

    return lineage;
  }
}
