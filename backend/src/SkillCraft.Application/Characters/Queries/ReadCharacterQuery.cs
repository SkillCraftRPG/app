using MediatR;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Characters;

namespace SkillCraft.Application.Characters.Queries;

/// <exception cref="PermissionDeniedException"></exception>
public record ReadCharacterQuery(Guid Id) : Activity, IRequest<CharacterModel?>;

internal class ReadCharacterQueryHandler : IRequestHandler<ReadCharacterQuery, CharacterModel?>
{
  private readonly ICharacterQuerier _characterQuerier;
  private readonly IPermissionService _permissionService;

  public ReadCharacterQueryHandler(ICharacterQuerier characterQuerier, IPermissionService permissionService)
  {
    _characterQuerier = characterQuerier;
    _permissionService = permissionService;
  }

  public async Task<CharacterModel?> Handle(ReadCharacterQuery query, CancellationToken cancellationToken)
  {
    await _permissionService.EnsureCanPreviewAsync(query, EntityType.Character, cancellationToken);

    return await _characterQuerier.ReadAsync(query.GetWorldId(), query.Id, cancellationToken);
  }
}
