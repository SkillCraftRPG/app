﻿using FluentValidation;
using MediatR;
using SkillCraft.Application.Permissions;
using SkillCraft.Application.Personalities.Validators;
using SkillCraft.Application.Storages;
using SkillCraft.Contracts.Personalities;
using SkillCraft.Domain;
using SkillCraft.Domain.Customizations;
using SkillCraft.Domain.Personalities;

namespace SkillCraft.Application.Personalities.Commands;

public record UpdatePersonalityCommand(Guid Id, UpdatePersonalityPayload Payload) : Activity, IRequest<PersonalityModel?>;

internal class UpdatePersonalityCommandHandler : PersonalityCommandHandler, IRequestHandler<UpdatePersonalityCommand, PersonalityModel?>
{
  private readonly IPermissionService _permissionService;
  private readonly IPersonalityQuerier _personalityQuerier;
  private readonly IPersonalityRepository _personalityRepository;

  public UpdatePersonalityCommandHandler(
    ICustomizationRepository customizationRepository,
    IPermissionService permissionService,
    IPersonalityQuerier personalityQuerier,
    IPersonalityRepository personalityRepository,
    IStorageService storageService) : base(customizationRepository, permissionService, personalityRepository, storageService)
  {
    _permissionService = permissionService;
    _personalityQuerier = personalityQuerier;
    _personalityRepository = personalityRepository;
  }

  public async Task<PersonalityModel?> Handle(UpdatePersonalityCommand command, CancellationToken cancellationToken)
  {
    UpdatePersonalityPayload payload = command.Payload;
    new UpdatePersonalityValidator().ValidateAndThrow(payload);

    PersonalityId id = new(command.GetWorldId(), command.Id);
    Personality? personality = await _personalityRepository.LoadAsync(id, cancellationToken);
    if (personality == null)
    {
      return null;
    }

    await _permissionService.EnsureCanUpdateAsync(command, personality.GetMetadata(), cancellationToken);

    if (!string.IsNullOrWhiteSpace(payload.Name))
    {
      personality.Name = new Name(payload.Name);
    }
    if (payload.Description != null)
    {
      personality.Description = Description.TryCreate(payload.Description.Value);
    }

    if (payload.Attribute != null)
    {
      personality.Attribute = payload.Attribute.Value;
    }
    if (payload.GiftId != null)
    {
      await SetGiftAsync(command, personality, payload.GiftId.Value, cancellationToken);
    }

    personality.Update(command.GetUserId());

    await SaveAsync(personality, cancellationToken);

    return await _personalityQuerier.ReadAsync(personality, cancellationToken);
  }
}
