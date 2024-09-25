using FluentValidation.Results;
using MediatR;
using Moq;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Languages;
using SkillCraft.Domain;
using SkillCraft.Domain.Languages;

namespace SkillCraft.Application.Languages.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class ReplaceLanguageCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<ILanguageQuerier> _languageQuerier = new();
  private readonly Mock<ILanguageRepository> _languageRepository = new();
  private readonly Mock<IPermissionService> _permissionService = new();
  private readonly Mock<ISender> _sender = new();

  private readonly ReplaceLanguageCommandHandler _handler;

  private readonly WorldMock _world = new();

  public ReplaceLanguageCommandHandlerTests()
  {
    _handler = new(_languageQuerier.Object, _languageRepository.Object, _permissionService.Object, _sender.Object);
  }

  [Fact(DisplayName = "It should replace an existing language.")]
  public async Task It_should_replace_an_existing_language()
  {
    Language language = new(_world.Id, new Name("common"), _world.OwnerId);
    _languageRepository.Setup(x => x.LoadAsync(language.Id, _cancellationToken)).ReturnsAsync(language);

    ReplaceLanguagePayload payload = new(" Commun ")
    {
      Description = "  Cette langue est parlée par tous les habitants de l’Ouespéro.  ",
      Script = " Alphabet latin ",
      TypicalSpeakers = " Humains "
    };
    ReplaceLanguageCommand command = new(language.Id.ToGuid(), payload, Version: null);
    command.Contextualize();

    LanguageModel model = new();
    _languageQuerier.Setup(x => x.ReadAsync(language, _cancellationToken)).ReturnsAsync(model);

    LanguageModel? result = await _handler.Handle(command, _cancellationToken);
    Assert.NotNull(result);
    Assert.Same(model, result);

    _permissionService.Verify(x => x.EnsureCanUpdateAsync(
      command,
      It.Is<EntityMetadata>(y => y.WorldId == _world.Id && y.Key.Type == EntityType.Language && y.Key.Id == language.Id.ToGuid() && y.Size > 0),
      _cancellationToken), Times.Once);

    _sender.Verify(x => x.Send(It.Is<SaveLanguageCommand>(y => y.Language.Equals(language)
      && y.Language.Name.Value == payload.Name.Trim()
      && y.Language.Description != null && y.Language.Description.Value == payload.Description.Trim()
      && y.Language.Script != null && y.Language.Script.Value == payload.Script.Trim()
      && y.Language.TypicalSpeakers != null && y.Language.TypicalSpeakers.Value == payload.TypicalSpeakers.Trim()), _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should return null when the language could not be found.")]
  public async Task It_should_return_null_when_the_language_could_not_be_found()
  {
    ReplaceLanguagePayload payload = new("Commun");
    ReplaceLanguageCommand command = new(Guid.Empty, payload, Version: null);

    Assert.Null(await _handler.Handle(command, _cancellationToken));
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    ReplaceLanguagePayload payload = new("    ");
    ReplaceLanguageCommand command = new(Guid.Empty, payload, Version: null);

    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await _handler.Handle(command, _cancellationToken));

    ValidationFailure error = Assert.Single(exception.Errors);
    Assert.Equal("NotEmptyValidator", error.ErrorCode);
    Assert.Equal("Name", error.PropertyName);
    Assert.Equal(payload.Name, error.AttemptedValue);
  }

  [Fact(DisplayName = "It should update an existing language from a reference.")]
  public async Task It_should_update_an_existing_language_from_a_reference()
  {
    Language reference = new(_world.Id, new Name("common"), _world.OwnerId);
    long version = reference.Version;
    _languageRepository.Setup(x => x.LoadAsync(reference.Id, version, _cancellationToken)).ReturnsAsync(reference);

    Language language = new(_world.Id, reference.Name, _world.OwnerId, reference.Id);
    _languageRepository.Setup(x => x.LoadAsync(language.Id, _cancellationToken)).ReturnsAsync(language);

    Description description = new("  Cette langue est parlée par tous les habitants de l’Ouespéro.  ");
    language.Description = description;
    language.Update(_world.OwnerId);

    ReplaceLanguagePayload payload = new(" Commun ")
    {
      Description = "    ",
      Script = " Alphabet latin ",
      TypicalSpeakers = " Humains "
    };
    ReplaceLanguageCommand command = new(language.Id.ToGuid(), payload, version);
    command.Contextualize();

    LanguageModel model = new();
    _languageQuerier.Setup(x => x.ReadAsync(language, _cancellationToken)).ReturnsAsync(model);

    LanguageModel? result = await _handler.Handle(command, _cancellationToken);
    Assert.NotNull(result);
    Assert.Same(model, result);

    _permissionService.Verify(x => x.EnsureCanUpdateAsync(
      command,
      It.Is<EntityMetadata>(y => y.WorldId == _world.Id && y.Key.Type == EntityType.Language && y.Key.Id == language.Id.ToGuid() && y.Size > 0),
      _cancellationToken), Times.Once);

    _sender.Verify(x => x.Send(It.Is<SaveLanguageCommand>(y => y.Language.Equals(language)
      && y.Language.Name.Value == payload.Name.Trim()
      && y.Language.Description == description
      && y.Language.Script != null && y.Language.Script.Value == payload.Script.Trim()
      && y.Language.TypicalSpeakers != null && y.Language.TypicalSpeakers.Value == payload.TypicalSpeakers.Trim()), _cancellationToken), Times.Once);
  }
}
