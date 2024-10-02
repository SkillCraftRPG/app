﻿using FluentValidation;
using MediatR;
using SkillCraft.Application.Permissions;
using SkillCraft.Application.Personalities.Validators;
using SkillCraft.Contracts.Personalities;
using SkillCraft.Domain;
using SkillCraft.Domain.Customizations;
using SkillCraft.Domain.Personalities;

namespace SkillCraft.Application.Personalities.Commands;

public record ReplacePersonalityCommand(Guid Id, ReplacePersonalityPayload Payload, long? Version) : Activity, IRequest<PersonalityModel?>;

internal class ReplacePersonalityCommandHandler : IRequestHandler<ReplacePersonalityCommand, PersonalityModel?>
{
  private readonly IPermissionService _permissionService;
  private readonly IPersonalityQuerier _personalityQuerier;
  private readonly IPersonalityRepository _personalityRepository;
  private readonly ISender _sender;

  public ReplacePersonalityCommandHandler(
    IPermissionService permissionService,
    IPersonalityQuerier personalityQuerier,
    IPersonalityRepository personalityRepository,
    ISender sender)
  {
    _permissionService = permissionService;
    _personalityQuerier = personalityQuerier;
    _personalityRepository = personalityRepository;
    _sender = sender;
  }

  public async Task<PersonalityModel?> Handle(ReplacePersonalityCommand command, CancellationToken cancellationToken)
  {
    ReplacePersonalityPayload payload = command.Payload;
    new ReplacePersonalityValidator().ValidateAndThrow(payload);

    PersonalityId id = new(command.GetWorldId(), command.Id);
    Personality? personality = await _personalityRepository.LoadAsync(id, cancellationToken);
    if (personality == null)
    {
      return null;
    }

    await _permissionService.EnsureCanUpdateAsync(command, personality.GetMetadata(), cancellationToken);

    Personality? reference = null;
    if (command.Version.HasValue)
    {
      reference = await _personalityRepository.LoadAsync(id, command.Version.Value, cancellationToken);
    }
    reference ??= personality;

    Name name = new(payload.Name);
    if (name != reference.Name)
    {
      personality.Name = name;
    }
    Description? description = Description.TryCreate(payload.Description);
    if (description != reference.Description)
    {
      personality.Description = description;
    }

    if (payload.Attribute != reference.Attribute)
    {
      personality.Attribute = payload.Attribute;
    }
    CustomizationId? giftId = payload.GiftId.HasValue ? new(personality.WorldId, payload.GiftId.Value) : null;
    if (giftId != reference.GiftId)
    {
      await _sender.Send(new SetGiftCommand(command, personality, payload.GiftId), cancellationToken);
    }

    personality.Update(command.GetUserId());
    await _sender.Send(new SavePersonalityCommand(personality), cancellationToken);

    return await _personalityQuerier.ReadAsync(personality, cancellationToken);
  }
}
