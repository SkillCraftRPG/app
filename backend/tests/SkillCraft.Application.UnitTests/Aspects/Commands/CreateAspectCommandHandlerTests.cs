using Logitar.Portal.Contracts.Users;
using MediatR;
using Moq;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Aspects;
using SkillCraft.Domain;
using SkillCraft.Domain.Aspects;
using SkillCraft.Domain.Worlds;
using Attribute = SkillCraft.Contracts.Attribute;

namespace SkillCraft.Application.Aspects.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class CreateAspectCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IAspectQuerier> _aspectQuerier = new();
  private readonly Mock<IPermissionService> _permissionService = new();
  private readonly Mock<ISender> _sender = new();

  private readonly CreateAspectCommandHandler _handler;

  private readonly User _user;
  private readonly World _world;

  public CreateAspectCommandHandlerTests()
  {
    _handler = new(_aspectQuerier.Object, _permissionService.Object, _sender.Object);

    _user = new UserMock();
    _world = new(new Slug("ungar"), new UserId(_user.Id));
  }

  [Fact(DisplayName = "It should create a new aspect.")]
  public async Task It_should_create_a_new_aspect()
  {
    CreateAspectPayload payload = new(" Œil-de-lynx ")
    {
      Description = "    ",
      Attributes = new AttributesModel
      {
        Mandatory1 = Attribute.Coordination,
        Mandatory2 = Attribute.Sensitivity,
        Optional1 = Attribute.Intellect,
        Optional2 = Attribute.Presence
      },
      Skills = new SkillsModel
      {
        Discounted1 = Skill.Orientation,
        Discounted2 = Skill.Perception
      }
    };
    CreateAspectCommand command = new(payload);
    command.Contextualize(_user, _world);

    AspectModel model = new();
    _aspectQuerier.Setup(x => x.ReadAsync(It.IsAny<Aspect>(), _cancellationToken)).ReturnsAsync(model);

    AspectModel result = await _handler.Handle(command, _cancellationToken);
    Assert.Same(result, model);

    _permissionService.Verify(x => x.EnsureCanCreateAsync(command, EntityType.Aspect, _cancellationToken), Times.Once);

    _sender.Verify(x => x.Send(It.Is<SaveAspectCommand>(y => y.Aspect.WorldId == _world.Id
      && y.Aspect.Name.Value == payload.Name.Trim()
      && y.Aspect.Description == null
      && y.Aspect.Attributes.Mandatory1 == payload.Attributes.Mandatory1
      && y.Aspect.Attributes.Mandatory2 == payload.Attributes.Mandatory2
      && y.Aspect.Attributes.Optional1 == payload.Attributes.Optional1
      && y.Aspect.Attributes.Optional2 == payload.Attributes.Optional2
      && y.Aspect.Skills.Discounted1 == payload.Skills.Discounted1
      && y.Aspect.Skills.Discounted2 == payload.Skills.Discounted2), _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    CreateAspectPayload payload = new("Œil-de-lynx")
    {
      Attributes = new AttributesModel
      {
        Mandatory1 = Attribute.Coordination,
        Mandatory2 = Attribute.Sensitivity,
        Optional1 = Attribute.Coordination,
        Optional2 = Attribute.Presence
      },
      Skills = new SkillsModel
      {
        Discounted1 = Skill.Orientation,
        Discounted2 = Skill.Orientation
      }
    };
    CreateAspectCommand command = new(payload);

    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await _handler.Handle(command, _cancellationToken));

    Assert.Equal(4, exception.Errors.Count());
    Assert.Contains(exception.Errors, e => e.ErrorCode == "AttributesValidator" && e.PropertyName == "Mandatory1" && e.AttemptedValue.Equals(Attribute.Coordination));
    Assert.Contains(exception.Errors, e => e.ErrorCode == "AttributesValidator" && e.PropertyName == "Optional1" && e.AttemptedValue.Equals(Attribute.Coordination));
    Assert.Contains(exception.Errors, e => e.ErrorCode == "SkillsValidator" && e.PropertyName == "Discounted1" && e.AttemptedValue.Equals(Skill.Orientation));
    Assert.Contains(exception.Errors, e => e.ErrorCode == "SkillsValidator" && e.PropertyName == "Discounted2" && e.AttemptedValue.Equals(Skill.Orientation));
  }
}
