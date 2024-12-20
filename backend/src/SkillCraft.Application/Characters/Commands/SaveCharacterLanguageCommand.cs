﻿using FluentValidation;
using MediatR;
using SkillCraft.Application.Characters.Validators;
using SkillCraft.Application.Permissions;
using SkillCraft.Application.Storages;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Characters;
using SkillCraft.Domain;
using SkillCraft.Domain.Characters;
using SkillCraft.Domain.Languages;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Application.Characters.Commands;

public record SaveCharacterLanguageCommand(Guid CharacterId, Guid LanguageId, CharacterLanguagePayload Payload) : Activity, IRequest<CharacterModel?>;

/// <exception cref="NotEnoughAvailableStorageException"></exception>
/// <exception cref="PermissionDeniedException"></exception>
/// <exception cref="ValidationException"></exception>
internal class SaveCharacterLanguageCommandHandler : IRequestHandler<SaveCharacterLanguageCommand, CharacterModel?>
{
  private readonly ICharacterQuerier _characterQuerier;
  private readonly ICharacterRepository _characterRepository;
  private readonly ILanguageRepository _languageRepository;
  private readonly IPermissionService _permissionService;
  private readonly ISender _sender;

  public SaveCharacterLanguageCommandHandler(
    ICharacterQuerier characterQuerier,
    ICharacterRepository characterRepository,
    ILanguageRepository languageRepository,
    IPermissionService permissionService,
    ISender sender)
  {
    _characterQuerier = characterQuerier;
    _characterRepository = characterRepository;
    _languageRepository = languageRepository;
    _permissionService = permissionService;
    _sender = sender;
  }

  public async Task<CharacterModel?> Handle(SaveCharacterLanguageCommand command, CancellationToken cancellationToken)
  {
    CharacterLanguagePayload payload = command.Payload;
    new CharacterLanguageValidator().ValidateAndThrow(payload);

    WorldId worldId = command.GetWorldId();
    CharacterId characterId = new(worldId, command.CharacterId);
    LanguageId languageId = new(worldId, command.LanguageId);

    Character? character = await _characterRepository.LoadAsync(characterId, cancellationToken);
    Language? language = await _languageRepository.LoadAsync(languageId, cancellationToken);
    if (character == null || language == null)
    {
      return null;
    }

    await _permissionService.EnsureCanUpdateAsync(command, character.GetMetadata(), cancellationToken);
    await _permissionService.EnsureCanPreviewAsync(command, EntityType.Language, cancellationToken);

    Description? notes = Description.TryCreate(payload.Notes);
    character.SetLanguage(language, notes, command.GetUserId());

    await _sender.Send(new SaveCharacterCommand(character), cancellationToken);

    return await _characterQuerier.ReadAsync(character, cancellationToken);
  }
}
