using FluentValidation;
using MediatR;
using SkillCraft.Application.Customizations;
using SkillCraft.Application.Permissions;
using SkillCraft.Application.Personalities.Validators;
using SkillCraft.Application.Storages;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Personalities;
using SkillCraft.Domain;
using SkillCraft.Domain.Customizations;
using SkillCraft.Domain.Personalities;

namespace SkillCraft.Application.Personalities.Commands;

public record CreateOrReplacePersonalityResult(PersonalityModel? Personality = null, bool Created = false);

/// <exception cref="CustomizationIsNotGiftException"></exception>
/// <exception cref="CustomizationNotFoundException"></exception>
/// <exception cref="NotEnoughAvailableStorageException"></exception>
/// <exception cref="PermissionDeniedException"></exception>
/// <exception cref="ValidationException"></exception>
public record CreateOrReplacePersonalityCommand(Guid? Id, CreateOrReplacePersonalityPayload Payload, long? Version) : Activity, IRequest<CreateOrReplacePersonalityResult>;

internal class CreateOrReplacePersonalityCommandHandler : IRequestHandler<CreateOrReplacePersonalityCommand, CreateOrReplacePersonalityResult>
{
  private readonly IPermissionService _permissionService;
  private readonly IPersonalityQuerier _personalityQuerier;
  private readonly IPersonalityRepository _personalityRepository;
  private readonly ISender _sender;

  public CreateOrReplacePersonalityCommandHandler(
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

  public async Task<CreateOrReplacePersonalityResult> Handle(CreateOrReplacePersonalityCommand command, CancellationToken cancellationToken)
  {
    new CreateOrReplacePersonalityValidator().ValidateAndThrow(command.Payload);

    Personality? personality = await FindAsync(command, cancellationToken);
    bool created = false;
    if (personality == null)
    {
      if (command.Version.HasValue)
      {
        return new CreateOrReplacePersonalityResult();
      }

      personality = await CreateAsync(command, cancellationToken);
      created = true;
    }
    else
    {
      await ReplaceAsync(command, personality, cancellationToken);
    }

    await _sender.Send(new SavePersonalityCommand(personality), cancellationToken);

    PersonalityModel model = await _personalityQuerier.ReadAsync(personality, cancellationToken);
    return new CreateOrReplacePersonalityResult(model, created);
  }

  private async Task<Personality?> FindAsync(CreateOrReplacePersonalityCommand command, CancellationToken cancellationToken)
  {
    if (!command.Id.HasValue)
    {
      return null;
    }

    PersonalityId id = new(command.GetWorldId(), command.Id.Value);
    return await _personalityRepository.LoadAsync(id, cancellationToken);
  }

  private async Task<Personality> CreateAsync(CreateOrReplacePersonalityCommand command, CancellationToken cancellationToken)
  {
    await _permissionService.EnsureCanPreviewAsync(command, EntityType.Customization, cancellationToken);
    await _permissionService.EnsureCanCreateAsync(command, EntityType.Personality, cancellationToken);

    CreateOrReplacePersonalityPayload payload = command.Payload;
    UserId userId = command.GetUserId();

    Personality personality = new(command.GetWorldId(), new Name(payload.Name), userId, command.Id)
    {
      Description = Description.TryCreate(payload.Description),
      Attribute = payload.Attribute
    };

    await _sender.Send(new SetGiftCommand(personality, payload.GiftId), cancellationToken);

    personality.Update(userId);

    return personality;
  }

  private async Task ReplaceAsync(CreateOrReplacePersonalityCommand command, Personality personality, CancellationToken cancellationToken)
  {
    await _permissionService.EnsureCanPreviewAsync(command, EntityType.Customization, cancellationToken);
    await _permissionService.EnsureCanUpdateAsync(command, personality.GetMetadata(), cancellationToken);

    CreateOrReplacePersonalityPayload payload = command.Payload;
    UserId userId = command.GetUserId();

    Personality? reference = null;
    if (command.Version.HasValue)
    {
      reference = await _personalityRepository.LoadAsync(personality.Id, command.Version.Value, cancellationToken);
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
      await _sender.Send(new SetGiftCommand(personality, payload.GiftId), cancellationToken);
    }

    personality.Update(userId);
  }
}
