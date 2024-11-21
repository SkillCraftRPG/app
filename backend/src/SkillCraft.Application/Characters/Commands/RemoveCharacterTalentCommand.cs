using MediatR;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts.Characters;
using SkillCraft.Domain.Characters;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Application.Characters.Commands;

/// <exception cref="PermissionDeniedException"></exception>
public record RemoveCharacterTalentCommand(Guid CharacterId, Guid RelationId) : Activity, IRequest<CharacterModel?>;

internal class RemoveCharacterTalentCommandHandler : IRequestHandler<RemoveCharacterTalentCommand, CharacterModel?>
{
  private readonly ICharacterQuerier _characterQuerier;
  private readonly ICharacterRepository _characterRepository;
  private readonly IPermissionService _permissionService;
  private readonly ISender _sender;

  public RemoveCharacterTalentCommandHandler(
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

  public async Task<CharacterModel?> Handle(RemoveCharacterTalentCommand command, CancellationToken cancellationToken)
  {
    WorldId worldId = command.GetWorldId();
    CharacterId characterId = new(worldId, command.CharacterId);
    Character? character = await _characterRepository.LoadAsync(characterId, cancellationToken);
    if (character == null)
    {
      return null;
    }

    await _permissionService.EnsureCanUpdateAsync(command, character.GetMetadata(), cancellationToken);

    character.RemoveTalent(command.RelationId, command.GetUserId());

    await _sender.Send(new SaveCharacterCommand(character), cancellationToken);

    return await _characterQuerier.ReadAsync(character, cancellationToken);
  }
}
