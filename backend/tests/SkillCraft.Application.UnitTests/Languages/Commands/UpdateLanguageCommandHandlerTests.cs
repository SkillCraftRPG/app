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
    command.Contextualize(_world);

    Assert.Null(await _handler.Handle(command, _cancellationToken));
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    UpdateLanguagePayload payload = new()
    {
      Name = RandomStringGenerator.GetString(Name.MaximumLength + 1)
    };
    UpdateLanguageCommand command = new(Guid.Empty, payload);

    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await _handler.Handle(command, _cancellationToken));

    ValidationFailure error = Assert.Single(exception.Errors);
    Assert.Equal("MaximumLengthValidator", error.ErrorCode);
    Assert.Equal("Name", error.PropertyName);
    Assert.Equal(payload.Name, error.AttemptedValue);
  }

  [Fact(DisplayName = "It should update an existing language.")]
  public async Task It_should_update_an_existing_language()
  {
    Language language = new(_world.Id, new Name("orrinique"), _world.OwnerId)
    {
      Description = new Description("Cette langue est issue des Orrins. Elle est très utilisée en Ouespéro en raison de l’histoire des Orrins. En effet, il s’agit d’une des premières civilisations du continent, leur culture et leur influence sont étendues aux confins de l’Ouespéro.")
    };
    language.Update(_world.OwnerId);
    _languageRepository.Setup(x => x.LoadAsync(language.Id, _cancellationToken)).ReturnsAsync(language);

    UpdateLanguagePayload payload = new()
    {
      Name = " Orrinique ",
      Description = new Change<string>("    "),
      Script = new Change<string>(" Orrinique "),
      TypicalSpeakers = new Change<string>("  Chalites, Minotaures, Orrins, Satyres, Sophitéons  ")
    };
    UpdateLanguageCommand command = new(language.EntityId, payload);
    command.Contextualize(_world);

    LanguageModel model = new();
    _languageQuerier.Setup(x => x.ReadAsync(language, _cancellationToken)).ReturnsAsync(model);

    LanguageModel? result = await _handler.Handle(command, _cancellationToken);
    Assert.NotNull(result);
    Assert.Same(model, result);

    _permissionService.Verify(x => x.EnsureCanUpdateAsync(command,
      It.Is<EntityMetadata>(y => y.WorldId == _world.Id && y.Type == EntityType.Language && y.Id == language.EntityId && y.Size > 0),
      _cancellationToken), Times.Once);

    Assert.NotNull(payload.Script.Value);
    Assert.NotNull(payload.TypicalSpeakers.Value);
    _sender.Verify(x => x.Send(
      It.Is<SaveLanguageCommand>(y => y.Language.Equals(language)
        && y.Language.Name.Value == payload.Name.Trim()
        && y.Language.Description == null
        && y.Language.Script != null && y.Language.Script.Value == payload.Script.Value.Trim()
        && y.Language.TypicalSpeakers != null && y.Language.TypicalSpeakers.Value == payload.TypicalSpeakers.Value.Trim()),
      _cancellationToken), Times.Once);
  }
}
