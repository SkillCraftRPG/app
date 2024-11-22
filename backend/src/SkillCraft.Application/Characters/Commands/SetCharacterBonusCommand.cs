﻿using FluentValidation;
using MediatR;
using SkillCraft.Application.Characters.Validators;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts.Characters;
using SkillCraft.Domain;
using SkillCraft.Domain.Characters;

namespace SkillCraft.Application.Characters.Commands;

public record SetCharacterBonusCommand(Guid CharacterId, Guid? BonusId, BonusPayload Payload) : Activity, IRequest<CharacterModel?>;

internal class SetCharacterBonusCommandHandler : IRequestHandler<SetCharacterBonusCommand, CharacterModel?>
{
  private readonly ICharacterQuerier _characterQuerier;
  private readonly ICharacterRepository _characterRepository;
  private readonly IPermissionService _permissionService;
  private readonly ISender _sender;

  public SetCharacterBonusCommandHandler(
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

  public async Task<CharacterModel?> Handle(SetCharacterBonusCommand command, CancellationToken cancellationToken)
  {
    BonusPayload payload = command.Payload;
    new BonusValidator().ValidateAndThrow(payload);

    CharacterId characterId = new(command.GetWorldId(), command.CharacterId);
    Character? character = await _characterRepository.LoadAsync(characterId, cancellationToken);
    if (character == null)
    {
      return null;
    }

    await _permissionService.EnsureCanUpdateAsync(command, character.GetMetadata(), cancellationToken);

    Name? precision = Name.TryCreate(payload.Precision);
    Description? notes = Description.TryCreate(payload.Notes);
    Bonus bonus = new(payload.Category, payload.Target, payload.Value, payload.IsTemporary, precision, notes);
    UserId userId = command.GetUserId();
    if (command.BonusId.HasValue)
    {
      character.SetBonus(command.BonusId.Value, bonus, userId);
    }
    else
    {
      character.AddBonus(bonus, userId);
    }

    await _sender.Send(new SaveCharacterCommand(character), cancellationToken);

    return await _characterQuerier.ReadAsync(character, cancellationToken);
  }
}
