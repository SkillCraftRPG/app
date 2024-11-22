using MediatR;
using Moq;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Characters;
using SkillCraft.Domain;
using SkillCraft.Domain.Characters;
using SkillCraft.Domain.Languages;

namespace SkillCraft.Application.Characters.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class SetCharacterLanguageCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<ICharacterQuerier> _characterQuerier = new();
  private readonly Mock<ICharacterRepository> _characterRepository = new();
  private readonly Mock<ILanguageRepository> _languageRepository = new();
  private readonly Mock<IPermissionService> _permissionService = new();
  private readonly Mock<ISender> _sender = new();

  private readonly SetCharacterLanguageCommandHandler _handler;

  private readonly WorldMock _world = new();

  public SetCharacterLanguageCommandHandlerTests()
  {
    _handler = new(_characterQuerier.Object, _characterRepository.Object, _languageRepository.Object, _permissionService.Object, _sender.Object);
  }

  [Fact(DisplayName = "It should add a new character language.")]
  public async Task It_should_add_a_character_language()
  {
    Character character = new CharacterBuilder(_world).Build();
    _characterRepository.Setup(x => x.LoadAsync(character.Id, _cancellationToken)).ReturnsAsync(character);

    Language language = new(_world.Id, new Name("Rénon"), _world.OwnerId);
    _languageRepository.Setup(x => x.LoadAsync(language.Id, _cancellationToken)).ReturnsAsync(language);

    CharacterLanguagePayload payload = new()
    {
      Notes = "  Level 1  "
    };
    SetCharacterLanguageCommand command = new(character.EntityId, language.EntityId, payload);
    command.Contextualize(_world);

    CharacterModel model = new();
    _characterQuerier.Setup(x => x.ReadAsync(character, _cancellationToken)).ReturnsAsync(model);

    CharacterModel? result = await _handler.Handle(command, _cancellationToken);
    Assert.NotNull(result);
    Assert.Same(model, result);

    KeyValuePair<LanguageId, LanguageMetadata> relation = Assert.Single(character.Languages);
    Assert.Equal(language.Id, relation.Key);
    Assert.Equal(payload.Notes.Trim(), relation.Value.Notes?.Value);

    _permissionService.Verify(x => x.EnsureCanUpdateAsync(command, character.GetMetadata(), _cancellationToken), Times.Once);
    _permissionService.Verify(x => x.EnsureCanPreviewAsync(command, EntityType.Language, _cancellationToken), Times.Once);

    _sender.Verify(x => x.Send(It.Is<SaveCharacterCommand>(y => y.Character.Equals(character)), _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should replace an existing character language.")]
  public async Task It_should_replace_an_existing_character_language()
  {
    Language language = new(_world.Id, new Name("Rénon"), _world.OwnerId);
    _languageRepository.Setup(x => x.LoadAsync(language.Id, _cancellationToken)).ReturnsAsync(language);

    Character character = new CharacterBuilder(_world).Build();
    character.SetLanguage(language, notes: null, _world.OwnerId);
    _characterRepository.Setup(x => x.LoadAsync(character.Id, _cancellationToken)).ReturnsAsync(character);

    CharacterLanguagePayload payload = new()
    {
      Notes = "  Level 1  "
    };
    SetCharacterLanguageCommand command = new(character.EntityId, language.EntityId, payload);
    command.Contextualize(_world);

    CharacterModel model = new();
    _characterQuerier.Setup(x => x.ReadAsync(character, _cancellationToken)).ReturnsAsync(model);

    CharacterModel? result = await _handler.Handle(command, _cancellationToken);
    Assert.NotNull(result);
    Assert.Same(model, result);

    KeyValuePair<LanguageId, LanguageMetadata> relation = Assert.Single(character.Languages);
    Assert.Equal(language.Id, relation.Key);
    Assert.Equal(payload.Notes.Trim(), relation.Value.Notes?.Value);

    _permissionService.Verify(x => x.EnsureCanUpdateAsync(command, character.GetMetadata(), _cancellationToken), Times.Once);
    _permissionService.Verify(x => x.EnsureCanPreviewAsync(command, EntityType.Language, _cancellationToken), Times.Once);

    _sender.Verify(x => x.Send(It.Is<SaveCharacterCommand>(y => y.Character.Equals(character)), _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should return null when the character could not be found.")]
  public async Task It_should_return_null_when_the_character_could_not_be_found()
  {
    CharacterLanguagePayload payload = new();
    SetCharacterLanguageCommand command = new(Guid.Empty, Guid.Empty, payload);
    command.Contextualize(_world);

    CharacterModel? character = await _handler.Handle(command, _cancellationToken);
    Assert.Null(character);
  }

  [Fact(DisplayName = "It should return null when the language could not be found.")]
  public async Task It_should_return_null_when_the_language_could_not_be_found()
  {
    Character character = new CharacterBuilder(_world).Build();
    _characterRepository.Setup(x => x.LoadAsync(character.Id, _cancellationToken)).ReturnsAsync(character);

    CharacterLanguagePayload payload = new();
    SetCharacterLanguageCommand command = new(character.EntityId, Guid.Empty, payload);
    command.Contextualize(_world);

    CharacterModel? result = await _handler.Handle(command, _cancellationToken);
    Assert.Null(result);
  }
}
