using FluentValidation.Results;
using Logitar.Portal.Contracts.Users;
using MediatR;
using Moq;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts.Languages;
using SkillCraft.Domain;
using SkillCraft.Domain.Languages;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Application.Languages.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class CreateLanguageCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<ILanguageQuerier> _languageQuerier = new();
  private readonly Mock<IPermissionService> _permissionService = new();
  private readonly Mock<ISender> _sender = new();

  private readonly CreateLanguageCommandHandler _handler;

  private readonly User _user;
  private readonly World _world;

  public CreateLanguageCommandHandlerTests()
  {
    _handler = new(_languageQuerier.Object, _permissionService.Object, _sender.Object);

    _user = new UserMock();
    _world = new(new Slug("ungar"), new UserId(_user.Id));
  }

  [Fact(DisplayName = "It should create a new language.")]
  public async Task It_should_create_a_new_language()
  {
    CreateLanguagePayload payload = new(" Commun ")
    {
      Description = "    ",
      Script = " Alphabet latin ",
      TypicalSpeakers = " Humains "
    };
    CreateLanguageCommand command = new(payload);
    command.Contextualize(_user, _world);

    LanguageModel model = new();
    _languageQuerier.Setup(x => x.ReadAsync(It.IsAny<Language>(), _cancellationToken)).ReturnsAsync(model);

    LanguageModel result = await _handler.Handle(command, _cancellationToken);
    Assert.Same(result, model);

    _permissionService.Verify(x => x.EnsureCanCreateAsync(command, EntityType.Language, _cancellationToken), Times.Once);

    _sender.Verify(x => x.Send(It.Is<SaveLanguageCommand>(y => y.Language.WorldId == _world.Id
      && y.Language.Name.Value == payload.Name.Trim()
      && y.Language.Description == null
      && y.Language.Script != null && y.Language.Script.Value == payload.Script.Trim()
      && y.Language.TypicalSpeakers != null && y.Language.TypicalSpeakers.Value == payload.TypicalSpeakers.Trim()), _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    CreateLanguagePayload payload = new("    ");
    CreateLanguageCommand command = new(payload);

    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await _handler.Handle(command, _cancellationToken));

    ValidationFailure error = Assert.Single(exception.Errors);
    Assert.Equal("NotEmptyValidator", error.ErrorCode);
    Assert.Equal("Name", error.PropertyName);
    Assert.Equal(payload.Name, error.AttemptedValue);
  }
}
