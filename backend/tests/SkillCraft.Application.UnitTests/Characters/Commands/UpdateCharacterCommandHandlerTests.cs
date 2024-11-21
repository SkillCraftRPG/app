using MediatR;
using Moq;
using SkillCraft.Application.Permissions;
using SkillCraft.Domain.Characters;

namespace SkillCraft.Application.Characters.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class UpdateCharacterCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<ICharacterQuerier> _characterQuerier = new();
  private readonly Mock<ICharacterRepository> _characterRepository = new();
  private readonly Mock<IPermissionService> _permissionService = new();
  private readonly Mock<ISender> _sender = new();

  private readonly UpdateCharacterCommandHandler _handler;

  public UpdateCharacterCommandHandlerTests()
  {
    _handler = new(_characterQuerier.Object, _characterRepository.Object, _permissionService.Object, _sender.Object);
  }

  // TODO(fpion): implement
}
