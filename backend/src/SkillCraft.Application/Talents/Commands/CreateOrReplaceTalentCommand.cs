using FluentValidation;
using MediatR;
using SkillCraft.Application.Permissions;
using SkillCraft.Application.Storages;
using SkillCraft.Application.Talents.Validators;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Talents;
using SkillCraft.Domain;
using SkillCraft.Domain.Talents;

namespace SkillCraft.Application.Talents.Commands;

public record CreateOrReplaceTalentResult(TalentModel? Talent = null, bool Created = false);

/// <exception cref="InvalidRequiredTalentTierException"></exception>
/// <exception cref="NotEnoughAvailableStorageException"></exception>
/// <exception cref="PermissionDeniedException"></exception>
/// <exception cref="TalentNotFoundException"></exception>
/// <exception cref="TalentSkillAlreadyExistingException"></exception>
/// <exception cref="ValidationException"></exception>
public record CreateOrReplaceTalentCommand(Guid? Id, CreateOrReplaceTalentPayload Payload, long? Version) : Activity, IRequest<CreateOrReplaceTalentResult>;

internal class CreateOrReplaceTalentCommandHandler : IRequestHandler<CreateOrReplaceTalentCommand, CreateOrReplaceTalentResult>
{
  private readonly IPermissionService _permissionService;
  private readonly ISender _sender;
  private readonly ITalentQuerier _talentQuerier;
  private readonly ITalentRepository _talentRepository;

  public CreateOrReplaceTalentCommandHandler(
    IPermissionService permissionService,
    ISender sender,
    ITalentQuerier talentQuerier,
    ITalentRepository talentRepository)
  {
    _permissionService = permissionService;
    _sender = sender;
    _talentQuerier = talentQuerier;
    _talentRepository = talentRepository;
  }

  public async Task<CreateOrReplaceTalentResult> Handle(CreateOrReplaceTalentCommand command, CancellationToken cancellationToken)
  {
    new CreateOrReplaceTalentValidator().ValidateAndThrow(command.Payload);

    Talent? talent = await FindAsync(command, cancellationToken);
    bool created = false;
    if (talent == null)
    {
      if (command.Version.HasValue)
      {
        return new CreateOrReplaceTalentResult();
      }

      talent = await CreateAsync(command, cancellationToken);
      created = true;
    }
    else
    {
      await ReplaceAsync(command, talent, cancellationToken);
    }

    await _sender.Send(new SaveTalentCommand(talent), cancellationToken);

    TalentModel model = await _talentQuerier.ReadAsync(talent, cancellationToken);
    return new CreateOrReplaceTalentResult(model, created);
  }

  private async Task<Talent?> FindAsync(CreateOrReplaceTalentCommand command, CancellationToken cancellationToken)
  {
    if (!command.Id.HasValue)
    {
      return null;
    }

    TalentId id = new(command.GetWorldId(), command.Id.Value);
    return await _talentRepository.LoadAsync(id, cancellationToken);
  }

  private async Task<Talent> CreateAsync(CreateOrReplaceTalentCommand command, CancellationToken cancellationToken)
  {
    await _permissionService.EnsureCanCreateAsync(command, EntityType.Talent, cancellationToken);

    CreateOrReplaceTalentPayload payload = command.Payload;
    UserId userId = command.GetUserId();

    Talent talent = new(command.GetWorldId(), payload.Tier, new Name(payload.Name), userId, command.Id)
    {
      Description = Description.TryCreate(payload.Description),
      AllowMultiplePurchases = payload.AllowMultiplePurchases,
      Skill = payload.Skill
    };
    if (payload.RequiredTalentId.HasValue)
    {
      await _sender.Send(new SetRequiredTalentCommand(talent, payload.RequiredTalentId.Value), cancellationToken);
    }

    talent.Update(userId);

    return talent;
  }

  private async Task ReplaceAsync(CreateOrReplaceTalentCommand command, Talent talent, CancellationToken cancellationToken)
  {
    await _permissionService.EnsureCanUpdateAsync(command, talent.GetMetadata(), cancellationToken);

    CreateOrReplaceTalentPayload payload = command.Payload;
    UserId userId = command.GetUserId();

    Talent? reference = null;
    if (command.Version.HasValue)
    {
      reference = await _talentRepository.LoadAsync(talent.Id, command.Version.Value, cancellationToken);
    }
    reference ??= talent;

    Name name = new(payload.Name);
    if (name != reference.Name)
    {
      talent.Name = name;
    }
    Description? description = Description.TryCreate(payload.Description);
    if (description != reference.Description)
    {
      talent.Description = description;
    }

    if (payload.AllowMultiplePurchases != reference.AllowMultiplePurchases)
    {
      talent.AllowMultiplePurchases = payload.AllowMultiplePurchases;
    }
    TalentId? requiredTalentId = payload.RequiredTalentId.HasValue ? new(talent.WorldId, payload.RequiredTalentId.Value) : null;
    if (requiredTalentId != reference.RequiredTalentId)
    {
      await _sender.Send(new SetRequiredTalentCommand(talent, payload.RequiredTalentId), cancellationToken);
    }
    if (payload.Skill != reference.Skill)
    {
      talent.Skill = payload.Skill;
    }

    talent.Update(userId);
  }
}
