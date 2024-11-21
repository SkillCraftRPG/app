using FluentValidation;
using MediatR;
using SkillCraft.Application.Characters.Validators;
using SkillCraft.Application.Permissions;
using SkillCraft.Application.Storages;
using SkillCraft.Application.Talents;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Characters;
using SkillCraft.Domain;
using SkillCraft.Domain.Characters;
using SkillCraft.Domain.Talents;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Application.Characters.Commands;

/// <exception cref="NotEnoughAvailableStorageException"></exception>
/// <exception cref="NotEnoughRemainingTalentPointsException"></exception>
/// <exception cref="PermissionDeniedException"></exception>
/// <exception cref="TalentCannotBePurchasedMultipleTimesException"></exception>
/// <exception cref="TalentMaximumCostExceededException"></exception>
/// <exception cref="TalentNotFoundException"></exception>
/// <exception cref="TalentTierCannotExceedCharacterTierException"></exception>
/// <exception cref="ValidationException"></exception>
public record SetCharacterTalentCommand(Guid CharacterId, Guid? RelationId, CharacterTalentPayload Payload) : Activity, IRequest<CharacterModel?>;

internal class SetCharacterTalentCommandHandler : IRequestHandler<SetCharacterTalentCommand, CharacterModel?>
{
  private readonly ICharacterQuerier _characterQuerier;
  private readonly ICharacterRepository _characterRepository;
  private readonly IPermissionService _permissionService;
  private readonly ISender _sender;
  private readonly ITalentRepository _talentRepository;

  public SetCharacterTalentCommandHandler(
    ICharacterQuerier characterQuerier,
    ICharacterRepository characterRepository,
    IPermissionService permissionService,
    ISender sender,
    ITalentRepository talentRepository)
  {
    _characterQuerier = characterQuerier;
    _characterRepository = characterRepository;
    _permissionService = permissionService;
    _sender = sender;
    _talentRepository = talentRepository;
  }

  public async Task<CharacterModel?> Handle(SetCharacterTalentCommand command, CancellationToken cancellationToken)
  {
    CharacterTalentPayload payload = command.Payload;
    new CharacterTalentValidator().ValidateAndThrow(payload);

    WorldId worldId = command.GetWorldId();
    CharacterId characterId = new(worldId, command.CharacterId);
    Character? character = await _characterRepository.LoadAsync(characterId, cancellationToken);
    if (character == null)
    {
      return null;
    }

    await _permissionService.EnsureCanUpdateAsync(command, character.GetMetadata(), cancellationToken);
    await _permissionService.EnsureCanPreviewAsync(command, EntityType.Talent, cancellationToken);

    SetTalentOptions options = new()
    {
      Cost = payload.Cost,
      Precision = Name.TryCreate(payload.Precision),
      Notes = Description.TryCreate(payload.Notes)
    };
    UserId userId = command.GetUserId();
    if (command.RelationId.HasValue)
    {
      TalentId talentId = (character.Talents.TryGetValue(command.RelationId.Value, out CharacterTalent? relation)
        ? relation.Id
        : (TalentId?)null) ?? new(worldId, payload.TalentId);
      Talent talent = await _talentRepository.LoadAsync(talentId, cancellationToken)
        ?? throw new TalentNotFoundException(talentId, nameof(payload.TalentId));
      character.SetTalent(command.RelationId.Value, talent, options, userId);
    }
    else
    {
      TalentId talentId = new(worldId, payload.TalentId);
      Talent talent = await _talentRepository.LoadAsync(talentId, cancellationToken)
        ?? throw new TalentNotFoundException(talentId, nameof(payload.TalentId));
      character.AddTalent(talent, options, userId);
    }

    await _sender.Send(new SaveCharacterCommand(character), cancellationToken);

    return await _characterQuerier.ReadAsync(character, cancellationToken);
  }
}
