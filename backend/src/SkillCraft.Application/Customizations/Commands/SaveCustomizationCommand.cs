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

public record SaveCustomizationResult(CustomizationModel? Customization = null, bool Created = false);

public record SaveCustomizationCommand(Guid? Id, SaveCustomizationPayload Payload, long? Version) : Activity, IRequest<SaveCustomizationResult>;

internal class SaveCustomizationCommandHandler : CustomizationCommandHandler, IRequestHandler<SaveCustomizationCommand, SaveCustomizationResult>
{
  private readonly ICustomizationQuerier _customizationQuerier;
  private readonly ICustomizationRepository _customizationRepository;
  private readonly IPermissionService _permissionService;

  public SaveCustomizationCommandHandler(
    ICustomizationQuerier customizationQuerier,
    ICustomizationRepository customizationRepository,
    IPermissionService permissionService,
    IStorageService storageService) : base(customizationRepository, storageService)
  {
    _customizationQuerier = customizationQuerier;
    _customizationRepository = customizationRepository;
    _permissionService = permissionService;
  }

  public async Task<SaveCustomizationResult> Handle(SaveCustomizationCommand command, CancellationToken cancellationToken)
  {
    new SaveCustomizationValidator().ValidateAndThrow(command.Payload);

    Customization? customization = await FindAsync(command, cancellationToken);
    bool created = false;
    if (customization == null)
    {
      if (command.Version.HasValue)
      {
        return new SaveCustomizationResult();
      }

      customization = await CreateAsync(command, cancellationToken);
      created = true;
    }
    else
    {
      await ReplaceAsync(command, customization, cancellationToken);
    }

    await SaveAsync(customization, cancellationToken);

    CustomizationModel model = await _customizationQuerier.ReadAsync(customization, cancellationToken);
    return new SaveCustomizationResult(model, created);
  }

  private async Task<Customization?> FindAsync(SaveCustomizationCommand command, CancellationToken cancellationToken)
  {
    if (!command.Id.HasValue)
    {
      return null;
    }

    CustomizationId id = new(command.GetWorldId(), command.Id.Value);
    return await _customizationRepository.LoadAsync(id, cancellationToken);
  }

  private async Task<Customization> CreateAsync(SaveCustomizationCommand command, CancellationToken cancellationToken)
  {
    await _permissionService.EnsureCanCreateAsync(command, EntityType.Customization, cancellationToken);

    SaveCustomizationPayload payload = command.Payload;
    UserId userId = command.GetUserId();

    Customization customization = new(command.GetWorldId(), payload.Type, new Name(payload.Name), userId, command.Id)
    {
      Description = Description.TryCreate(payload.Description)
    };
    customization.Update(userId);

    return customization;
  }

  private async Task ReplaceAsync(SaveCustomizationCommand command, Customization customization, CancellationToken cancellationToken)
  {
    await _permissionService.EnsureCanUpdateAsync(command, customization.GetMetadata(), cancellationToken);

    SaveCustomizationPayload payload = command.Payload;
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
