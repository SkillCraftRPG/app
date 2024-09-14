using FluentValidation.Results;
using Logitar.Portal.Contracts.Users;
using MediatR;
using Moq;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Castes;
using SkillCraft.Domain;
using SkillCraft.Domain.Castes;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Application.Castes.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class CreateCasteCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<ICasteQuerier> _casteQuerier = new();
  private readonly Mock<IPermissionService> _permissionService = new();
  private readonly Mock<ISender> _sender = new();

  private readonly CreateCasteCommandHandler _handler;

  private readonly User _user;
  private readonly World _world;

  public CreateCasteCommandHandlerTests()
  {
    _handler = new(_casteQuerier.Object, _permissionService.Object, _sender.Object);

    _user = new UserMock();
    _world = new(new Slug("ungar"), new UserId(_user.Id));
  }

  [Fact(DisplayName = "It should create a new caste.")]
  public async Task It_should_create_a_new_caste()
  {
    CreateCastePayload payload = new(" Artisan ")
    {
      Description = "    ",
      Skill = Skill.Knowledge,
      WealthRoll = "8d6",
      Traits = [new TraitPayload { Name = "Professionnel" }]
    };
    CreateCasteCommand command = new(payload);
    command.Contextualize(_user, _world);

    CasteModel model = new();
    _casteQuerier.Setup(x => x.ReadAsync(It.IsAny<Caste>(), _cancellationToken)).ReturnsAsync(model);

    CasteModel result = await _handler.Handle(command, _cancellationToken);
    Assert.Same(result, model);

    _permissionService.Verify(x => x.EnsureCanCreateAsync(command, EntityType.Caste, _cancellationToken), Times.Once);

    _sender.Verify(x => x.Send(It.Is<SaveCasteCommand>(y => y.Caste.WorldId == _world.Id
      && y.Caste.Name.Value == payload.Name.Trim()
      && y.Caste.Description == null
      && y.Caste.Skill == payload.Skill
      && y.Caste.WealthRoll != null && y.Caste.WealthRoll.Value == payload.WealthRoll
      && y.Caste.Traits.Single().Value.Name.Value == payload.Traits.Single().Name), _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    CreateCastePayload payload = new("Artisan")
    {
      WealthRoll = "8d6+7"
    };
    CreateCasteCommand command = new(payload);

    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await _handler.Handle(command, _cancellationToken));

    ValidationFailure error = Assert.Single(exception.Errors);
    Assert.Equal("RollValidator", error.ErrorCode);
    Assert.Equal("WealthRoll", error.PropertyName);
    Assert.Equal(payload.WealthRoll, error.AttemptedValue);
  }
}
