using FluentValidation;
using MediatR;
using SkillCraft.Application.Customizations;
using SkillCraft.Application.Natures.Validators;
using SkillCraft.Application.Permissions;
using SkillCraft.Application.Storages;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Natures;
using SkillCraft.Domain;
using SkillCraft.Domain.Customizations;
using SkillCraft.Domain.Natures;

namespace SkillCraft.Application.Natures.Commands;

public record CreateOrReplaceNatureResult(NatureModel? Nature = null, bool Created = false);

/// <exception cref="CustomizationIsNotGiftException"></exception>
/// <exception cref="CustomizationNotFoundException"></exception>
/// <exception cref="NotEnoughAvailableStorageException"></exception>
/// <exception cref="PermissionDeniedException"></exception>
/// <exception cref="ValidationException"></exception>
public record CreateOrReplaceNatureCommand(Guid? Id, CreateOrReplaceNaturePayload Payload, long? Version) : Activity, IRequest<CreateOrReplaceNatureResult>;

internal class CreateOrReplaceNatureCommandHandler : IRequestHandler<CreateOrReplaceNatureCommand, CreateOrReplaceNatureResult>
{
  private readonly ICustomizationRepository _customizationRepository;
  private readonly INatureQuerier _natureQuerier;
  private readonly INatureRepository _natureRepository;
  private readonly IPermissionService _permissionService;
  private readonly ISender _sender;

  public CreateOrReplaceNatureCommandHandler(
    ICustomizationRepository customizationRepository,
    INatureQuerier natureQuerier,
    INatureRepository natureRepository,
    IPermissionService permissionService,
    ISender sender)
  {
    _customizationRepository = customizationRepository;
    _natureQuerier = natureQuerier;
    _natureRepository = natureRepository;
    _permissionService = permissionService;
    _sender = sender;
  }

  public async Task<CreateOrReplaceNatureResult> Handle(CreateOrReplaceNatureCommand command, CancellationToken cancellationToken)
  {
    new CreateOrReplaceNatureValidator().ValidateAndThrow(command.Payload);

    Nature? nature = await FindAsync(command, cancellationToken);
    bool created = false;
    if (nature == null)
    {
      if (command.Version.HasValue)
      {
        return new CreateOrReplaceNatureResult();
      }

      nature = await CreateAsync(command, cancellationToken);
      created = true;
    }
    else
    {
      await ReplaceAsync(command, nature, cancellationToken);
    }

    await _sender.Send(new SaveNatureCommand(nature), cancellationToken);

    NatureModel model = await _natureQuerier.ReadAsync(nature, cancellationToken);
    return new CreateOrReplaceNatureResult(model, created);
  }

  private async Task<Nature?> FindAsync(CreateOrReplaceNatureCommand command, CancellationToken cancellationToken)
  {
    if (!command.Id.HasValue)
    {
      return null;
    }

    NatureId id = new(command.GetWorldId(), command.Id.Value);
    return await _natureRepository.LoadAsync(id, cancellationToken);
  }

  private async Task<Nature> CreateAsync(CreateOrReplaceNatureCommand command, CancellationToken cancellationToken)
  {
    await _permissionService.EnsureCanPreviewAsync(command, EntityType.Customization, cancellationToken);
    await _permissionService.EnsureCanCreateAsync(command, EntityType.Nature, cancellationToken);

    CreateOrReplaceNaturePayload payload = command.Payload;
    UserId userId = command.GetUserId();

    Nature nature = new(command.GetWorldId(), new Name(payload.Name), userId, command.Id)
    {
      Description = Description.TryCreate(payload.Description),
      Attribute = payload.Attribute
    };

    if (payload.GiftId.HasValue)
    {
      CustomizationId giftId = new(nature.WorldId, payload.GiftId.Value);
      Customization gift = await _customizationRepository.LoadAsync(giftId, cancellationToken)
        ?? throw new CustomizationNotFoundException(giftId, nameof(payload.GiftId));
      nature.SetGift(gift);
    }

    nature.Update(userId);

    return nature;
  }

  private async Task ReplaceAsync(CreateOrReplaceNatureCommand command, Nature nature, CancellationToken cancellationToken)
  {
    await _permissionService.EnsureCanPreviewAsync(command, EntityType.Customization, cancellationToken);
    await _permissionService.EnsureCanUpdateAsync(command, nature.GetMetadata(), cancellationToken);

    CreateOrReplaceNaturePayload payload = command.Payload;
    UserId userId = command.GetUserId();

    Nature? reference = null;
    if (command.Version.HasValue)
    {
      reference = await _natureRepository.LoadAsync(nature.Id, command.Version.Value, cancellationToken);
    }
    reference ??= nature;

    Name name = new(payload.Name);
    if (name != reference.Name)
    {
      nature.Name = name;
    }
    Description? description = Description.TryCreate(payload.Description);
    if (description != reference.Description)
    {
      nature.Description = description;
    }

    if (payload.Attribute != reference.Attribute)
    {
      nature.Attribute = payload.Attribute;
    }
    CustomizationId? giftId = payload.GiftId.HasValue ? new(nature.WorldId, payload.GiftId.Value) : null;
    if (giftId != reference.GiftId)
    {
      Customization? gift = null;
      if (giftId.HasValue)
      {
        gift = await _customizationRepository.LoadAsync(giftId.Value, cancellationToken)
          ?? throw new CustomizationNotFoundException(giftId.Value, nameof(payload.GiftId));
      }
      nature.SetGift(gift);
    }

    nature.Update(userId);
  }
}
