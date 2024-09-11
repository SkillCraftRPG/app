using FluentValidation;
using MediatR;
using SkillCraft.Application.Worlds.Validators;
using SkillCraft.Contracts.Worlds;
using SkillCraft.Domain;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Application.Worlds.Commands;

public record CreateWorldCommand(CreateWorldPayload Payload) : Activity, IRequest<WorldModel>;

internal class CreateWorldCommandHandler : IRequestHandler<CreateWorldCommand, WorldModel>
{
  private readonly ISender _sender;
  private readonly IWorldQuerier _worldQuerier;

  public CreateWorldCommandHandler(ISender sender, IWorldQuerier worldQuerier)
  {
    _sender = sender;
    _worldQuerier = worldQuerier;
  }

  public async Task<WorldModel> Handle(CreateWorldCommand command, CancellationToken cancellationToken)
  {
    CreateWorldPayload payload = command.Payload;
    new CreateWorldValidator().ValidateAndThrow(payload);

    // TODO(fpion): permissions

    UserId userId = command.GetUserId();
    World world = new(new Slug(payload.Slug), userId)
    {
      Name = Name.TryCreate(payload.Name),
      Description = Description.TryCreate(payload.Description)
    };
    world.Update(userId);

    await _sender.Send(new SaveWorldCommand(world), cancellationToken);

    return await _worldQuerier.ReadAsync(world, cancellationToken);
  }
}
