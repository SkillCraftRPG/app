using FluentValidation;
using MediatR;
using SkillCraft.Application.Characters.Validators;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts.Characters;
using SkillCraft.Domain.Characters;

namespace SkillCraft.Application.Characters.Commands;

public record LevelUpCharacterCommand(Guid Id, LevelUpCharacterPayload Payload) : Activity, IRequest<CharacterModel?>;

internal class LevelUpCharacterCommandHandler : IRequestHandler<LevelUpCharacterCommand, CharacterModel?>
{
  private readonly ICharacterQuerier _characterQuerier;
  private readonly ICharacterRepository _characterRepository;
  private readonly IPermissionService _permissionService;
  private readonly ISender _sender;

  public LevelUpCharacterCommandHandler(
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

  public async Task<CharacterModel?> Handle(LevelUpCharacterCommand command, CancellationToken cancellationToken)
  {
    LevelUpCharacterPayload payload = command.Payload;
    new LevelUpCharacterValidator().ValidateAndThrow(payload);

    CharacterId characterId = new(command.GetWorldId(), command.Id);
    Character? character = await _characterRepository.LoadAsync(characterId, cancellationToken);
    if (character == null)
    {
      return null;
    }

    await _permissionService.EnsureCanUpdateAsync(command, character.GetMetadata(), cancellationToken);

    character.LevelUp(payload.Attribute, command.GetUserId());

    await _sender.Send(new SaveCharacterCommand(character), cancellationToken);

    return await _characterQuerier.ReadAsync(character, cancellationToken);
  }
}
