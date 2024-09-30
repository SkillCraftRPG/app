using FluentValidation;
using MediatR;
using SkillCraft.Application.Aspects.Validators;
using SkillCraft.Application.Permissions;
using SkillCraft.Application.Storages;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Aspects;
using SkillCraft.Domain;
using SkillCraft.Domain.Aspects;

namespace SkillCraft.Application.Aspects.Commands;

public record SaveAspectResult(AspectModel? Aspect = null, bool Created = false);

public record SaveAspectCommand(Guid? Id, SaveAspectPayload Payload, long? Version) : Activity, IRequest<SaveAspectResult>;

internal class SaveAspectCommandHandler : IRequestHandler<SaveAspectCommand, SaveAspectResult>
{
  private readonly IAspectQuerier _aspectQuerier;
  private readonly IAspectRepository _aspectRepository;
  private readonly IPermissionService _permissionService;
  private readonly IStorageService _storageService;

  public SaveAspectCommandHandler(
    IAspectQuerier aspectQuerier,
    IAspectRepository aspectRepository,
    IPermissionService permissionService,
    IStorageService storageService)
  {
    _aspectQuerier = aspectQuerier;
    _aspectRepository = aspectRepository;
    _permissionService = permissionService;
    _storageService = storageService;
  }

  public async Task<SaveAspectResult> Handle(SaveAspectCommand command, CancellationToken cancellationToken)
  {
    new SaveAspectValidator().ValidateAndThrow(command.Payload);

    Aspect? aspect = await FindAsync(command, cancellationToken);
    bool created = false;
    if (aspect == null)
    {
      if (command.Version.HasValue)
      {
        return new SaveAspectResult();
      }

      aspect = await CreateAsync(command, cancellationToken);
      created = true;
    }
    else
    {
      await ReplaceAsync(command, aspect, cancellationToken);
    }

    await SaveAsync(aspect, cancellationToken);

    AspectModel model = await _aspectQuerier.ReadAsync(aspect, cancellationToken);
    return new SaveAspectResult(model, created);
  }

  private async Task<Aspect?> FindAsync(SaveAspectCommand command, CancellationToken cancellationToken)
  {
    if (!command.Id.HasValue)
    {
      return null;
    }

    AspectId id = new(command.GetWorldId(), command.Id.Value);
    return await _aspectRepository.LoadAsync(id, cancellationToken);
  }

  private async Task<Aspect> CreateAsync(SaveAspectCommand command, CancellationToken cancellationToken)
  {
    await _permissionService.EnsureCanCreateAsync(command, EntityType.Aspect, cancellationToken);

    SaveAspectPayload payload = command.Payload;
    UserId userId = command.GetUserId();

    Aspect aspect = new(command.GetWorldId(), new Name(payload.Name), userId, command.Id)
    {
      Description = Description.TryCreate(payload.Description),
      Attributes = new AttributeSelection(payload.Attributes),
      Skills = new Skills(payload.Skills)
    };
    aspect.Update(userId);

    return aspect;
  }

  private async Task ReplaceAsync(SaveAspectCommand command, Aspect aspect, CancellationToken cancellationToken)
  {
    await _permissionService.EnsureCanUpdateAsync(command, aspect.GetMetadata(), cancellationToken);

    SaveAspectPayload payload = command.Payload;
    UserId userId = command.GetUserId();

    Aspect? reference = null;
    if (command.Version.HasValue)
    {
      reference = await _aspectRepository.LoadAsync(aspect.Id, command.Version.Value, cancellationToken);
    }
    reference ??= aspect;

    Name name = new(payload.Name);
    if (name != reference.Name)
    {
      aspect.Name = name;
    }
    Description? description = Description.TryCreate(payload.Description);
    if (description != reference.Description)
    {
      aspect.Description = description;
    }

    AttributeSelection attributes = new(payload.Attributes);
    if (attributes != reference.Attributes)
    {
      aspect.Attributes = attributes;
    }
    Skills skills = new(payload.Skills);
    if (skills != reference.Skills)
    {
      aspect.Skills = skills;
    }

    aspect.Update(userId);
  }

  private async Task SaveAsync(Aspect aspect, CancellationToken cancellationToken)
  {
    EntityMetadata entity = aspect.GetMetadata();
    await _storageService.EnsureAvailableAsync(entity, cancellationToken);

    await _aspectRepository.SaveAsync(aspect, cancellationToken);

    await _storageService.UpdateAsync(entity, cancellationToken);
  }
}
