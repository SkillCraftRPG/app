using FluentValidation.Results;
using Logitar.Portal.Contracts.Users;
using MediatR;
using Moq;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Educations;
using SkillCraft.Domain;
using SkillCraft.Domain.Educations;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Application.Educations.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class CreateEducationCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IEducationQuerier> _educationQuerier = new();
  private readonly Mock<IPermissionService> _permissionService = new();
  private readonly Mock<ISender> _sender = new();

  private readonly CreateEducationCommandHandler _handler;

  private readonly User _user;
  private readonly World _world;

  public CreateEducationCommandHandlerTests()
  {
    _handler = new(_educationQuerier.Object, _permissionService.Object, _sender.Object);

    _user = new UserMock();
    _world = new(new Slug("ungar"), new UserId(_user.Id));
  }

  [Fact(DisplayName = "It should create a new education.")]
  public async Task It_should_create_a_new_education()
  {
    CreateEducationPayload payload = new(" Classique ")
    {
      Description = "    ",
      Skill = Skill.Knowledge
    };
    CreateEducationCommand command = new(payload);
    command.Contextualize(_user, _world);

    EducationModel model = new();
    _educationQuerier.Setup(x => x.ReadAsync(It.Is<Education>(y => y.Name.Value == payload.Name.Trim()
      && y.Description == null && y.WorldId == command.GetWorldId()
    ), _cancellationToken)).ReturnsAsync(model);

    EducationModel result = await _handler.Handle(command, _cancellationToken);
    Assert.Same(result, model);

    _permissionService.Verify(x => x.EnsureCanCreateAsync(command, EntityType.Education, _cancellationToken), Times.Once);

    _sender.Verify(x => x.Send(It.Is<SaveEducationCommand>(y => y.Education.Name.Value == payload.Name.Trim()
      && y.Education.Description == null && y.Education.WorldId == command.GetWorldId()), _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    CreateEducationPayload payload = new("Classique")
    {
      WealthMultiplier = 0.0
    };
    CreateEducationCommand command = new(payload);

    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await _handler.Handle(command, _cancellationToken));

    ValidationFailure error = Assert.Single(exception.Errors);
    Assert.Equal("GreaterThanValidator", error.ErrorCode);
    Assert.Equal("WealthMultiplier.Value", error.PropertyName);
    Assert.Equal(payload.WealthMultiplier, error.AttemptedValue);
  }
}
