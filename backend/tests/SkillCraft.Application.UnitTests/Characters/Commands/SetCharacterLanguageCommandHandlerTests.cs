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

  private readonly SetCharacterLanguageCommandHandler _handler;

  private readonly WorldMock _world = new();

  public SetCharacterLanguageCommandHandlerTests()
  {
    _handler = new(_characterQuerier.Object, _characterRepository.Object, _languageRepository.Object, _permissionService.Object);
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

  [Fact(DisplayName = "It should set the character language.")]
  public async Task It_should_set_the_character_language()
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

    _characterRepository.Verify(x => x.SaveAsync(
      It.Is<Character>(y => y.Equals(character) && y.Languages.Count == 1
        && y.Languages.Any(l => l.Key == language.Id && l.Value.Notes != null && l.Value.Notes.Value == payload.Notes.Trim())),
      _cancellationToken), Times.Once);

    _permissionService.Verify(x => x.EnsureCanUpdateAsync(command, character.GetMetadata(), _cancellationToken), Times.Once);
    _permissionService.Verify(x => x.EnsureCanPreviewAsync(command, EntityType.Language, _cancellationToken), Times.Once);
  }
}
