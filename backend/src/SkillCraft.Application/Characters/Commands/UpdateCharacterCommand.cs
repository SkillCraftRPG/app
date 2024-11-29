using FluentValidation;
using MediatR;
using SkillCraft.Application.Characters.Validators;
using SkillCraft.Application.Permissions;
using SkillCraft.Application.Storages;
using SkillCraft.Contracts.Characters;
using SkillCraft.Domain;
using SkillCraft.Domain.Characters;

namespace SkillCraft.Application.Characters.Commands;

/// <exception cref="NotEnoughAvailableStorageException"></exception>
/// <exception cref="PermissionDeniedException"></exception>
/// <exception cref="ValidationException"></exception>
public record UpdateCharacterCommand(Guid Id, UpdateCharacterPayload Payload) : Activity, IRequest<CharacterModel?>;

internal class UpdateCharacterCommandHandler : IRequestHandler<UpdateCharacterCommand, CharacterModel?>
{
  private readonly ICharacterQuerier _characterQuerier;
  private readonly ICharacterRepository _characterRepository;
  private readonly IPermissionService _permissionService;
  private readonly ISender _sender;

  public UpdateCharacterCommandHandler(
    ICharacterQuerier characterQuerier,
    ICharacterRepository characterRepository,
    IPermissionService permissionService,
    ISender sender)
  {
    _characterQuerier = characterQuerier;
    _characterRepository = characterRepository;
    _permissionService = permissionService;
    _sender = sender;
  }

  public async Task<CharacterModel?> Handle(UpdateCharacterCommand command, CancellationToken cancellationToken)
  {
    UpdateCharacterPayload payload = command.Payload;
    new UpdateCharacterValidator().ValidateAndThrow(payload);

    CharacterId characterId = new(command.GetWorldId(), command.Id);
    Character? character = await _characterRepository.LoadAsync(characterId, cancellationToken);
    if (character == null)
    {
      return null;
    }

    await _permissionService.EnsureCanUpdateAsync(command, character.GetMetadata(), cancellationToken);

    if (!string.IsNullOrWhiteSpace(payload.Name))
    {
      character.Name = new Name(payload.Name);
    }
    if (payload.Player != null)
    {
      character.Player = PlayerName.TryCreate(payload.Player.Value);
    }

    if (payload.Height.HasValue)
    {
      character.Height = payload.Height.Value;
    }
    if (payload.Weight.HasValue)
    {
      character.Weight = payload.Weight.Value;
    }
    if (payload.Age.HasValue)
    {
      character.Age = payload.Age.Value;
    }

    if (payload.Experience.HasValue)
    {
      character.Experience = payload.Experience.Value;
    }
    if (payload.Vitality.HasValue)
    {
      character.Vitality = payload.Vitality.Value;
    }
    if (payload.Stamina.HasValue)
    {
      character.Stamina = payload.Stamina.Value;
    }
    if (payload.BloodAlcoholContent.HasValue)
    {
      character.BloodAlcoholContent = payload.BloodAlcoholContent.Value;
    }
    if (payload.Intoxication.HasValue)
    {
      character.Intoxication = payload.Intoxication.Value;
    }

    foreach (SkillRankModel skillRank in payload.SkillRanks)
    {
      character.SetSkillRank(skillRank.Skill, skillRank.Rank);
    }

    character.Update(command.GetUserId());
    await _sender.Send(new SaveCharacterCommand(character), cancellationToken);

    return await _characterQuerier.ReadAsync(character, cancellationToken);
  }
}
