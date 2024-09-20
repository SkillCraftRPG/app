using FluentValidation;
using MediatR;
using SkillCraft.Application.Characters.Creation;
using SkillCraft.Application.Characters.Validators;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts.Characters;
using SkillCraft.Domain;
using SkillCraft.Domain.Characters;
using SkillCraft.Domain.Customizations;
using SkillCraft.Domain.Lineages;
using SkillCraft.Domain.Personalities;

namespace SkillCraft.Application.Characters.Commands;

public record CreateCharacterCommand(CreateCharacterPayload Payload) : Activity, IRequest<CharacterModel>;

internal class CreateCharacterCommandHandler : IRequestHandler<CreateCharacterCommand, CharacterModel>
{
  private readonly ICharacterQuerier _characterQuerier;
  private readonly IPermissionService _permissionService;
  private readonly ISender _sender;

  public CreateCharacterCommandHandler(ICharacterQuerier characterQuerier, IPermissionService permissionService, ISender sender)
  {
    _characterQuerier = characterQuerier;
    _permissionService = permissionService;
    _sender = sender;
  }

  public async Task<CharacterModel> Handle(CreateCharacterCommand command, CancellationToken cancellationToken)
  {
    CreateCharacterPayload payload = command.Payload;
    new CreateCharacterValidator().ValidateAndThrow(payload);

    await _permissionService.EnsureCanCreateAsync(command, EntityType.Character, cancellationToken);

    Lineage lineage = await _sender.Send(new ResolveLineageQuery(command, payload.LineageId), cancellationToken);
    Personality personality = await _sender.Send(new ResolvePersonalityQuery(command, payload.PersonalityId), cancellationToken);
    IReadOnlyCollection<Customization> customizations = await _sender.Send(
      new ResolveCustomizationsQuery(command, personality, payload.CustomizationIds),
      cancellationToken);

    Character character = new(
      command.GetWorldId(),
      new Name(payload.Name),
      PlayerName.TryCreate(payload.Player),
      lineage,
      payload.Height,
      payload.Weight,
      payload.Age,
      personality,
      customizations,
      command.GetUserId());

    await _sender.Send(new SaveCharacterCommand(character), cancellationToken);

    return await _characterQuerier.ReadAsync(character, cancellationToken);
  }
}
