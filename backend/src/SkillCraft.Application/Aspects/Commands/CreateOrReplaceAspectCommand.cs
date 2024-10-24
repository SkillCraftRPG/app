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

public record CreateOrReplaceAspectResult(AspectModel? Aspect = null, bool Created = false);

/// <exception cref="NotEnoughAvailableStorageException"></exception>
/// <exception cref="PermissionDeniedException"></exception>
/// <exception cref="ValidationException"></exception>
public record CreateOrReplaceAspectCommand(Guid? Id, CreateOrReplaceAspectPayload Payload, long? Version) : Activity, IRequest<CreateOrReplaceAspectResult>;

internal class CreateOrReplaceAspectCommandHandler : IRequestHandler<CreateOrReplaceAspectCommand, CreateOrReplaceAspectResult>
{
  private readonly IAspectQuerier _aspectQuerier;
  private readonly IAspectRepository _aspectRepository;
  private readonly IPermissionService _permissionService;
  private readonly ISender _sender;

  public CreateOrReplaceAspectCommandHandler(
    IAspectQuerier aspectQuerier,
    IAspectRepository aspectRepository,
    IPermissionService permissionService,
    ISender sender)
  {
    _aspectQuerier = aspectQuerier;
    _aspectRepository = aspectRepository;
    _permissionService = permissionService;
    _sender = sender;
  }

  public async Task<CreateOrReplaceAspectResult> Handle(CreateOrReplaceAspectCommand command, CancellationToken cancellationToken)
  {
    new CreateOrReplaceAspectValidator().ValidateAndThrow(command.Payload);

    Aspect? aspect = await FindAsync(command, cancellationToken);
    bool created = false;
    if (aspect == null)
    {
      if (command.Version.HasValue)
      {
        return new CreateOrReplaceAspectResult();
      }

      aspect = await CreateAsync(command, cancellationToken);
      created = true;
    }
    else
    {
      await ReplaceAsync(command, aspect, cancellationToken);
    }

    await _sender.Send(new SaveAspectCommand(aspect), cancellationToken);

    AspectModel model = await _aspectQuerier.ReadAsync(aspect, cancellationToken);
    return new CreateOrReplaceAspectResult(model, created);
  }

  private async Task<Aspect?> FindAsync(CreateOrReplaceAspectCommand command, CancellationToken cancellationToken)
  {
    if (!command.Id.HasValue)
    {
      return null;
    }

    AspectId id = new(command.GetWorldId(), command.Id.Value);
    return await _aspectRepository.LoadAsync(id, cancellationToken);
  }

  private async Task<Aspect> CreateAsync(CreateOrReplaceAspectCommand command, CancellationToken cancellationToken)
  {
    await _permissionService.EnsureCanCreateAsync(command, EntityType.Aspect, cancellationToken);

    CreateOrReplaceAspectPayload payload = command.Payload;
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

  private async Task ReplaceAsync(CreateOrReplaceAspectCommand command, Aspect aspect, CancellationToken cancellationToken)
  {
    await _permissionService.EnsureCanUpdateAsync(command, aspect.GetMetadata(), cancellationToken);

    CreateOrReplaceAspectPayload payload = command.Payload;
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
}
