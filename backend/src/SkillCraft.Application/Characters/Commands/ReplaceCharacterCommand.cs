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
public record ReplaceCharacterCommand(Guid Id, ReplaceCharacterPayload Payload, long? Version) : Activity, IRequest<CharacterModel?>;

internal class ReplaceCharacterCommandHandler : IRequestHandler<ReplaceCharacterCommand, CharacterModel?>
{
  private readonly ICharacterQuerier _characterQuerier;
  private readonly ICharacterRepository _characterRepository;
  private readonly IPermissionService _permissionService;
  private readonly ISender _sender;

  public ReplaceCharacterCommandHandler(
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

  public async Task<CharacterModel?> Handle(ReplaceCharacterCommand command, CancellationToken cancellationToken)
  {
    ReplaceCharacterPayload payload = command.Payload;
    new ReplaceCharacterValidator().ValidateAndThrow(payload);

    CharacterId characterId = new(command.GetWorldId(), command.Id);
    Character? character = await _characterRepository.LoadAsync(characterId, cancellationToken);
    if (character == null)
    {
      return null;
    }

    await _permissionService.EnsureCanUpdateAsync(command, character.GetMetadata(), cancellationToken);

    Character? reference = null;
    if (command.Version.HasValue)
    {
      reference = await _characterRepository.LoadAsync(character.Id, command.Version.Value, cancellationToken);
    }
    reference ??= character;

    Name name = new(payload.Name);
    if (reference.Name != name)
    {
      character.Name = name;
    }
    PlayerName? player = PlayerName.TryCreate(payload.Player);
    if (reference.Player != player)
    {
      character.Player = player;
    }

    if (reference.Height != payload.Height)
    {
      character.Height = payload.Height;
    }
    if (reference.Weight != payload.Weight)
    {
      character.Weight = payload.Weight;
    }
    if (reference.Age != payload.Age)
    {
      character.Age = payload.Age;
    }

    if (reference.Experience != payload.Experience)
    {
      character.Experience = payload.Experience;
    }
    if (reference.Vitality != payload.Vitality)
    {
      character.Vitality = payload.Vitality;
    }
    if (reference.Stamina != payload.Stamina)
    {
      character.Stamina = payload.Stamina;
    }
    if (reference.BloodAlcoholContent != payload.BloodAlcoholContent)
    {
      character.BloodAlcoholContent = payload.BloodAlcoholContent;
    }
    if (reference.Intoxication != payload.Intoxication)
    {
      character.Intoxication = payload.Intoxication;
    }

    character.Update(command.GetUserId());
    await _sender.Send(new SaveCharacterCommand(character), cancellationToken);

    return await _characterQuerier.ReadAsync(character, cancellationToken);
  }
}
