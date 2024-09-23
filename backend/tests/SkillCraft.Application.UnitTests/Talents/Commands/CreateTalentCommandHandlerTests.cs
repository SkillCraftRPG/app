using FluentValidation.Results;
using Logitar.Portal.Contracts.Users;
using MediatR;
using Moq;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Talents;
using SkillCraft.Domain;
using SkillCraft.Domain.Talents;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Application.Talents.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class CreateTalentCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IPermissionService> _permissionService = new();
  private readonly Mock<ISender> _sender = new();
  private readonly Mock<ITalentQuerier> _talentQuerier = new();

  private readonly CreateTalentCommandHandler _handler;

  private readonly User _user;
  private readonly World _world;

  public CreateTalentCommandHandlerTests()
  {
    _handler = new(_permissionService.Object, _sender.Object, _talentQuerier.Object);

    _user = new UserMock();
    _world = new(new Slug("ungar"), new UserId(_user.Id));
  }

  [Fact(DisplayName = "It should create a new talent with a required talent.")]
  public async Task It_should_create_a_new_talent_with_a_required_talent()
  {
    CreateTalentPayload payload = new(" Formation martiale ")
    {
      Description = "    ",
      RequiredTalentId = Guid.NewGuid()
    };
    CreateTalentCommand command = new(payload);
    command.Contextualize(_user, _world);

    TalentModel model = new();
    _talentQuerier.Setup(x => x.ReadAsync(It.IsAny<Talent>(), _cancellationToken)).ReturnsAsync(model);

    TalentModel result = await _handler.Handle(command, _cancellationToken);
    Assert.Same(result, model);

    _permissionService.Verify(x => x.EnsureCanCreateAsync(command, EntityType.Talent, _cancellationToken), Times.Once);

    _sender.Verify(x => x.Send(It.Is<SetRequiredTalentCommand>(y => y.Activity == command && y.Id == payload.RequiredTalentId), _cancellationToken), Times.Once);
    _sender.Verify(x => x.Send(It.Is<SaveTalentCommand>(y => y.Talent.WorldId == _world.Id
      && y.Talent.Tier == payload.Tier
      && y.Talent.Name.Value == payload.Name.Trim()
      && y.Talent.Description == null
      && y.Talent.AllowMultiplePurchases == payload.AllowMultiplePurchases), _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should create a new talent without a required talent.")]
  public async Task It_should_create_a_new_talent_without_a_required_talent()
  {
    CreateTalentPayload payload = new(" Mêlée ")
    {
      Description = "    ",
      AllowMultiplePurchases = true
    };
    CreateTalentCommand command = new(payload);
    command.Contextualize(_user, _world);

    TalentModel model = new();
    _talentQuerier.Setup(x => x.ReadAsync(It.IsAny<Talent>(), _cancellationToken)).ReturnsAsync(model);

    TalentModel result = await _handler.Handle(command, _cancellationToken);
    Assert.Same(result, model);

    _permissionService.Verify(x => x.EnsureCanCreateAsync(command, EntityType.Talent, _cancellationToken), Times.Once);

    _sender.Verify(x => x.Send(It.IsAny<SetRequiredTalentCommand>(), It.IsAny<CancellationToken>()), Times.Never);
    _sender.Verify(x => x.Send(It.Is<SaveTalentCommand>(y => y.Talent.WorldId == _world.Id
      && y.Talent.Tier == payload.Tier
      && y.Talent.Name.Value == payload.Name.Trim()
      && y.Talent.Description == null
      && y.Talent.AllowMultiplePurchases == payload.AllowMultiplePurchases
      && y.Talent.RequiredTalentId == null
      && y.Talent.Skill == Skill.Melee), _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    CreateTalentPayload payload = new("Acrobaties")
    {
      Tier = 10
    };
    CreateTalentCommand command = new(payload);

    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await _handler.Handle(command, _cancellationToken));

    ValidationFailure error = Assert.Single(exception.Errors);
    Assert.Equal("InclusiveBetweenValidator", error.ErrorCode);
    Assert.Equal("Tier", error.PropertyName);
    Assert.Equal(payload.Tier, error.AttemptedValue);
  }
}
