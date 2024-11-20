using MediatR;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts.Characters;
using SkillCraft.Domain.Characters;
using SkillCraft.Domain.Languages;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Application.Characters.Commands;

public record RemoveCharacterLanguageCommand(Guid CharacterId, Guid LanguageId) : Activity, IRequest<CharacterModel?>;

/// <exception cref="PermissionDeniedException"></exception>
internal class RemoveCharacterLanguageCommandHandler : IRequestHandler<RemoveCharacterLanguageCommand, CharacterModel?>
{
  private readonly ICharacterQuerier _characterQuerier;
  private readonly ICharacterRepository _characterRepository;
  private readonly IPermissionService _permissionService;

  public RemoveCharacterLanguageCommandHandler(ICharacterQuerier characterQuerier, ICharacterRepository characterRepository, IPermissionService permissionService)
  {
    _characterQuerier = characterQuerier;
    _characterRepository = characterRepository;
    _permissionService = permissionService;
  }

  public async Task<CharacterModel?> Handle(RemoveCharacterLanguageCommand command, CancellationToken cancellationToken)
  {
    WorldId worldId = command.GetWorldId();
    CharacterId characterId = new(worldId, command.CharacterId);
    Character? character = await _characterRepository.LoadAsync(characterId, cancellationToken);
    if (character == null)
    {
      return null;
    }

    await _permissionService.EnsureCanUpdateAsync(command, character.GetMetadata(), cancellationToken);

    LanguageId languageId = new(worldId, command.LanguageId);
    character.RemoveLanguage(languageId, command.GetUserId());

    await _characterRepository.SaveAsync(character, cancellationToken);

    return await _characterQuerier.ReadAsync(character, cancellationToken);
  }
}
