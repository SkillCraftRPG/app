using FluentValidation;
using MediatR;
using SkillCraft.Application.Permissions;
using SkillCraft.Application.Personalities.Validators;
using SkillCraft.Contracts.Personalities;
using SkillCraft.Domain;
using SkillCraft.Domain.Personalities;

namespace SkillCraft.Application.Personalities.Commands;

public record CreatePersonalityCommand(CreatePersonalityPayload Payload) : Activity, IRequest<PersonalityModel>;

internal class CreatePersonalityCommandHandler : IRequestHandler<CreatePersonalityCommand, PersonalityModel>
{
  private readonly IPermissionService _permissionService;
  private readonly IPersonalityQuerier _personalityQuerier;
  private readonly ISender _sender;

  public CreatePersonalityCommandHandler(IPermissionService permissionService, IPersonalityQuerier personalityQuerier, ISender sender)
  {
    _permissionService = permissionService;
    _personalityQuerier = personalityQuerier;
    _sender = sender;
  }

  public async Task<PersonalityModel> Handle(CreatePersonalityCommand command, CancellationToken cancellationToken)
  {
    CreatePersonalityPayload payload = command.Payload;
    new CreatePersonalityValidator().ValidateAndThrow(payload);

    await _permissionService.EnsureCanCreateAsync(command, EntityType.Personality, cancellationToken);

    UserId userId = command.GetUserId();
    Personality personality = new(command.GetWorldId(), new Name(payload.Name), userId)
    {
      Description = Description.TryCreate(payload.Description),
      Attribute = payload.Attribute
    };
    if (payload.GiftId.HasValue)
    {
      await _sender.Send(new SetGiftCommand(command, personality, payload.GiftId.Value), cancellationToken);
    }

    personality.Update(userId);
    await _sender.Send(new SavePersonalityCommand(personality), cancellationToken);

    return await _personalityQuerier.ReadAsync(personality, cancellationToken);
  }
}
