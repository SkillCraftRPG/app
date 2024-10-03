using FluentValidation;
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
public class CreateOrReplaceLanguageCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<ILanguageQuerier> _languageQuerier = new();
  private readonly Mock<ILanguageRepository> _languageRepository = new();
  private readonly Mock<IPermissionService> _permissionService = new();
  private readonly Mock<ISender> _sender = new();

  private readonly CreateOrReplaceLanguageCommandHandler _handler;

  private readonly WorldMock _world = new();
  private readonly Language _language;
  private readonly LanguageModel _model = new();

  public CreateOrReplaceLanguageCommandHandlerTests()
  {
    _handler = new(_languageQuerier.Object, _languageRepository.Object, _permissionService.Object, _sender.Object);

    _language = new(_world.Id, new Name("orrinique"), _world.OwnerId);
    _languageRepository.Setup(x => x.LoadAsync(_language.Id, _cancellationToken)).ReturnsAsync(_language);

    _languageQuerier.Setup(x => x.ReadAsync(It.IsAny<Language>(), _cancellationToken)).ReturnsAsync(_model);
  }

  [Theory(DisplayName = "It should create a new language.")]
  [InlineData(null)]
  [InlineData("e7eb1fea-31fb-42d0-b322-677ebe1aebd0")]
  public async Task It_should_create_a_new_language(string? idValue)
  {
    CreateOrReplaceLanguagePayload payload = new(" Orrinique ")
    {
      Description = "    ",
      Script = " Orrinique ",
      TypicalSpeakers = "  Chalites, Minotaures, Orrins, Satyres, Sophitéons  "
    };

    bool parsed = Guid.TryParse(idValue, out Guid id);
    CreateOrReplaceLanguageCommand command = new(parsed ? id : null, payload, Version: null);
    command.Contextualize(_world);

    CreateOrReplaceLanguageResult result = await _handler.Handle(command, _cancellationToken);
    Assert.Same(_model, result.Language);
    Assert.True(result.Created);

    _permissionService.Verify(x => x.EnsureCanCreateAsync(command, EntityType.Language, _cancellationToken), Times.Once);

    _sender.Verify(x => x.Send(
      It.Is<SaveLanguageCommand>(y => (!parsed || y.Language.EntityId == id)
        && y.Language.Name.Value == payload.Name.Trim()
        && y.Language.Description == null
        && y.Language.Script != null && y.Language.Script.Value == payload.Script.Trim()
        && y.Language.TypicalSpeakers != null && y.Language.TypicalSpeakers.Value == payload.TypicalSpeakers.Trim()),
      _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should replace an existing language.")]
  public async Task It_should_replace_an_existing_language()
  {
    CreateOrReplaceLanguagePayload payload = new(" Orrinique ")
    {
      Description = "  Cette langue est issue des Orrins. Elle est très utilisée en Ouespéro en raison de l’histoire des Orrins. En effet, il s’agit d’une des premières civilisations du continent, leur culture et leur influence sont étendues aux confins de l’Ouespéro.  ",
      Script = " Orrinique ",
      TypicalSpeakers = "  Chalites, Minotaures, Orrins, Satyres, Sophitéons  "
    };

    CreateOrReplaceLanguageCommand command = new(_language.EntityId, payload, Version: null);
    command.Contextualize(_world);

    CreateOrReplaceLanguageResult result = await _handler.Handle(command, _cancellationToken);
    Assert.Same(_model, result.Language);
    Assert.False(result.Created);

    _permissionService.Verify(x => x.EnsureCanUpdateAsync(
      command,
      It.Is<EntityMetadata>(y => y.WorldId == _world.Id && y.Type == EntityType.Language && y.Id == _language.EntityId && y.Size > 0),
      _cancellationToken), Times.Once);

    _sender.Verify(x => x.Send(
      It.Is<SaveLanguageCommand>(y => y.Language.Equals(_language)
        && y.Language.Name.Value == payload.Name.Trim()
        && y.Language.Description != null && y.Language.Description.Value == payload.Description.Trim()
        && y.Language.Script != null && y.Language.Script.Value == payload.Script.Trim()
        && y.Language.TypicalSpeakers != null && y.Language.TypicalSpeakers.Value == payload.TypicalSpeakers.Trim()),
      _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should return null when updating an language that does not exist.")]
  public async Task It_should_return_null_when_updating_an_language_that_does_not_exist()
  {
    CreateOrReplaceLanguageCommand command = new(Guid.Empty, new CreateOrReplaceLanguagePayload("Orrinique"), Version: 0);
    command.Contextualize(_world);

    CreateOrReplaceLanguageResult result = await _handler.Handle(command, _cancellationToken);
    Assert.Null(result.Language);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    CreateOrReplaceLanguagePayload payload = new();

    CreateOrReplaceLanguageCommand command = new(Id: null, payload, Version: null);
    var exception = await Assert.ThrowsAsync<ValidationException>(async () => await _handler.Handle(command, _cancellationToken));

    ValidationFailure error = Assert.Single(exception.Errors);
    Assert.Equal("NotEmptyValidator", error.ErrorCode);
    Assert.Equal("Name", error.PropertyName);
  }

  [Fact(DisplayName = "It should update an existing language.")]
  public async Task It_should_update_an_existing_language()
  {
    Language reference = new(_language.WorldId, _language.Name, _world.OwnerId, _language.EntityId)
    {
      Description = _language.Description,
      Script = _language.Script,
      TypicalSpeakers = _language.TypicalSpeakers
    };
    reference.Update(_world.OwnerId);
    _languageRepository.Setup(x => x.LoadAsync(reference.Id, reference.Version, _cancellationToken)).ReturnsAsync(reference);

    Description description = new("Cette langue est issue des Orrins. Elle est très utilisée en Ouespéro en raison de l’histoire des Orrins. En effet, il s’agit d’une des premières civilisations du continent, leur culture et leur influence sont étendues aux confins de l’Ouespéro.");
    _language.Description = description;
    _language.Update(_world.OwnerId);

    CreateOrReplaceLanguagePayload payload = new(" Orrinique ")
    {
      Description = "    ",
      Script = " Orrinique ",
      TypicalSpeakers = "  Chalites, Minotaures, Orrins, Satyres, Sophitéons  "
    };

    CreateOrReplaceLanguageCommand command = new(_language.EntityId, payload, reference.Version);
    command.Contextualize(_world);

    CreateOrReplaceLanguageResult result = await _handler.Handle(command, _cancellationToken);
    Assert.Same(_model, result.Language);
    Assert.False(result.Created);

    _permissionService.Verify(x => x.EnsureCanUpdateAsync(
      command,
      It.Is<EntityMetadata>(y => y.WorldId == _world.Id && y.Type == EntityType.Language && y.Id == _language.EntityId && y.Size > 0),
      _cancellationToken), Times.Once);

    _sender.Verify(x => x.Send(
      It.Is<SaveLanguageCommand>(y => y.Language.Equals(_language)
        && y.Language.Name.Value == payload.Name.Trim()
        && y.Language.Description == description
        && y.Language.Script != null && y.Language.Script.Value == payload.Script.Trim()
        && y.Language.TypicalSpeakers != null && y.Language.TypicalSpeakers.Value == payload.TypicalSpeakers.Trim()),
      _cancellationToken), Times.Once);
  }
}
