using FluentValidation;
using MediatR;
using SkillCraft.Application.Customizations;
using SkillCraft.Application.Permissions;
using SkillCraft.Application.Personalities.Validators;
using SkillCraft.Application.Storages;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Personalities;
using SkillCraft.Domain;
using SkillCraft.Domain.Personalities;

namespace SkillCraft.Application.Personalities.Commands;

/// <exception cref="CustomizationIsNotGiftException"></exception>
/// <exception cref="CustomizationNotFoundException"></exception>
/// <exception cref="NotEnoughAvailableStorageException"></exception>
/// <exception cref="PermissionDeniedException"></exception>
/// <exception cref="ValidationException"></exception>
public record UpdatePersonalityCommand(Guid Id, UpdatePersonalityPayload Payload) : Activity, IRequest<PersonalityModel?>;

internal class UpdatePersonalityCommandHandler : IRequestHandler<UpdatePersonalityCommand, PersonalityModel?>
{
  private readonly IPermissionService _permissionService;
  private readonly IPersonalityQuerier _personalityQuerier;
  private readonly IPersonalityRepository _personalityRepository;
  private readonly ISender _sender;

  public UpdatePersonalityCommandHandler(
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

    await _permissionService.EnsureCanPreviewAsync(command, EntityType.Customization, cancellationToken);
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
      await _sender.Send(new SetGiftCommand(personality, payload.GiftId.Value), cancellationToken);
    }

    personality.Update(command.GetUserId());

    await _sender.Send(new SavePersonalityCommand(personality), cancellationToken);

    return await _personalityQuerier.ReadAsync(personality, cancellationToken);
  }
}
