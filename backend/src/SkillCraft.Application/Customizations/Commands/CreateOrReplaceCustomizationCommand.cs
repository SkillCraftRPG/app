using FluentValidation;
using MediatR;
using SkillCraft.Application.Customizations.Validators;
using SkillCraft.Application.Permissions;
using SkillCraft.Application.Storages;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Customizations;
using SkillCraft.Domain;
using SkillCraft.Domain.Customizations;

namespace SkillCraft.Application.Customizations.Commands;

public record CreateOrReplaceCustomizationResult(CustomizationModel? Customization = null, bool Created = false);

/// <exception cref="NotEnoughAvailableStorageException"></exception>
/// <exception cref="PermissionDeniedException"></exception>
/// <exception cref="ValidationException"></exception>
public record CreateOrReplaceCustomizationCommand(Guid? Id, CreateOrReplaceCustomizationPayload Payload, long? Version) : Activity, IRequest<CreateOrReplaceCustomizationResult>;

internal class CreateOrReplaceCustomizationCommandHandler : IRequestHandler<CreateOrReplaceCustomizationCommand, CreateOrReplaceCustomizationResult>
{
  private readonly ICustomizationQuerier _customizationQuerier;
  private readonly ICustomizationRepository _customizationRepository;
  private readonly IPermissionService _permissionService;
  private readonly ISender _sender;

  public CreateOrReplaceCustomizationCommandHandler(
    ICustomizationQuerier customizationQuerier,
    ICustomizationRepository customizationRepository,
    IPermissionService permissionService,
    ISender sender)
  {
    _customizationQuerier = customizationQuerier;
    _customizationRepository = customizationRepository;
    _permissionService = permissionService;
    _sender = sender;
  }

  public async Task<CreateOrReplaceCustomizationResult> Handle(CreateOrReplaceCustomizationCommand command, CancellationToken cancellationToken)
  {
    new CreateOrReplaceCustomizationValidator().ValidateAndThrow(command.Payload);

    Customization? customization = await FindAsync(command, cancellationToken);
    bool created = false;
    if (customization == null)
    {
      if (command.Version.HasValue)
      {
        return new CreateOrReplaceCustomizationResult();
      }

      customization = await CreateAsync(command, cancellationToken);
      created = true;
    }
    else
    {
      await ReplaceAsync(command, customization, cancellationToken);
    }

    await _sender.Send(new SaveCustomizationCommand(customization), cancellationToken);

    CustomizationModel model = await _customizationQuerier.ReadAsync(customization, cancellationToken);
    return new CreateOrReplaceCustomizationResult(model, created);
  }

  private async Task<Customization?> FindAsync(CreateOrReplaceCustomizationCommand command, CancellationToken cancellationToken)
  {
    if (!command.Id.HasValue)
    {
      return null;
    }

    CustomizationId id = new(command.GetWorldId(), command.Id.Value);
    return await _customizationRepository.LoadAsync(id, cancellationToken);
  }

  private async Task<Customization> CreateAsync(CreateOrReplaceCustomizationCommand command, CancellationToken cancellationToken)
  {
    await _permissionService.EnsureCanCreateAsync(command, EntityType.Customization, cancellationToken);

    CreateOrReplaceCustomizationPayload payload = command.Payload;
    UserId userId = command.GetUserId();

    Customization customization = new(command.GetWorldId(), payload.Type, new Name(payload.Name), userId, command.Id)
    {
      Description = Description.TryCreate(payload.Description)
    };
    customization.Update(userId);

    return customization;
  }

  private async Task ReplaceAsync(CreateOrReplaceCustomizationCommand command, Customization customization, CancellationToken cancellationToken)
  {
    await _permissionService.EnsureCanUpdateAsync(command, customization.GetMetadata(), cancellationToken);

    CreateOrReplaceCustomizationPayload payload = command.Payload;
    UserId userId = command.GetUserId();

    Customization? reference = null;
    if (command.Version.HasValue)
    {
      reference = await _customizationRepository.LoadAsync(customization.Id, command.Version.Value, cancellationToken);
    }
    reference ??= customization;

    Name name = new(payload.Name);
    if (name != reference.Name)
    {
      customization.Name = name;
    }
    Description? description = Description.TryCreate(payload.Description);
    if (description != reference.Description)
    {
      customization.Description = description;
    }

    customization.Update(userId);
  }
}
