using FluentValidation.Results;
using Logitar.Security.Cryptography;
using MediatR;
using Moq;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Languages;
using SkillCraft.Domain;
using SkillCraft.Domain.Languages;

namespace SkillCraft.Application.Languages.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class UpdateLanguageCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<ILanguageQuerier> _languageQuerier = new();
  private readonly Mock<ILanguageRepository> _languageRepository = new();
  private readonly Mock<IPermissionService> _permissionService = new();
  private readonly Mock<ISender> _sender = new();

  private readonly UpdateLanguageCommandHandler _handler;

  private readonly WorldMock _world = new();

  public UpdateLanguageCommandHandlerTests()
  {
    _handler = new(_languageQuerier.Object, _languageRepository.Object, _permissionService.Object, _sender.Object);
  }

  [Fact(DisplayName = "It should return null when the language could not be found.")]
  public async Task It_should_return_null_when_the_language_could_not_be_found()
  {
    UpdateLanguagePayload payload = new();
    UpdateLanguageCommand command = new(Guid.Empty, payload);

    Assert.Null(await _handler.Handle(command, _cancellationToken));
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    UpdateLanguagePayload payload = new()
    {
      Script = new Change<string>(RandomStringGenerator.GetString(Script.MaximumLength + 1))
    };
    UpdateLanguageCommand command = new(Guid.Empty, payload);

    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await _handler.Handle(command, _cancellationToken));

    ValidationFailure error = Assert.Single(exception.Errors);
    Assert.Equal("MaximumLengthValidator", error.ErrorCode);
    Assert.Equal("Script.Value", error.PropertyName);
    Assert.Equal(payload.Script.Value, error.AttemptedValue);
  }

  [Fact(DisplayName = "It should update an existing language.")]
  public async Task It_should_update_an_existing_language()
  {
    Language language = new(_world.Id, new Name("common"), _world.OwnerId)
    {
      Description = new Description("Cette langue est parlée par tous les habitants de l’Ouespéro.")
    };
    language.Update(_world.OwnerId);
    _languageRepository.Setup(x => x.LoadAsync(language.Id, _cancellationToken)).ReturnsAsync(language);

    UpdateLanguagePayload payload = new()
    {
      Name = " Commun ",
      Description = new Change<string>("    "),
      Script = new Change<string>(" Alphabet latin "),
      TypicalSpeakers = new Change<string>(" Humains ")
    };
    UpdateLanguageCommand command = new(language.Id.ToGuid(), payload);
    command.Contextualize();

    LanguageModel model = new();
    _languageQuerier.Setup(x => x.ReadAsync(language, _cancellationToken)).ReturnsAsync(model);

    LanguageModel? result = await _handler.Handle(command, _cancellationToken);
    Assert.NotNull(result);
    Assert.Same(model, result);

    _permissionService.Verify(x => x.EnsureCanUpdateAsync(command,
      It.Is<EntityMetadata>(y => y.WorldId == _world.Id && y.Key.Type == EntityType.Language && y.Key.Id == language.Id.ToGuid() && y.Size > 0),
      _cancellationToken), Times.Once);

    Assert.NotNull(payload.Script.Value);
    Assert.NotNull(payload.TypicalSpeakers.Value);
    _sender.Verify(x => x.Send(It.Is<SaveLanguageCommand>(y => y.Language.Equals(language)
      && y.Language.Name.Value == payload.Name.Trim()
      && y.Language.Description == null
      && y.Language.Script != null && y.Language.Script.Value == payload.Script.Value.Trim()
      && y.Language.TypicalSpeakers != null && y.Language.TypicalSpeakers.Value == payload.TypicalSpeakers.Value.Trim()), _cancellationToken), Times.Once);
  }
}
