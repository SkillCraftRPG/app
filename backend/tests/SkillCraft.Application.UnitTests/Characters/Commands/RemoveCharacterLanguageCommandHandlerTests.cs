using MediatR;
using Moq;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts.Characters;
using SkillCraft.Domain;
using SkillCraft.Domain.Characters;
using SkillCraft.Domain.Languages;

namespace SkillCraft.Application.Characters.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class RemoveCharacterLanguageCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<ICharacterQuerier> _characterQuerier = new();
  private readonly Mock<ICharacterRepository> _characterRepository = new();
  private readonly Mock<IPermissionService> _permissionService = new();
  private readonly Mock<ISender> _sender = new();

  private readonly RemoveCharacterLanguageCommandHandler _handler;

  private readonly WorldMock _world = new();

  public RemoveCharacterLanguageCommandHandlerTests()
  {
    _handler = new(_characterQuerier.Object, _characterRepository.Object, _permissionService.Object, _sender.Object);
  }

  [Fact(DisplayName = "It should not do anything when the language could not be found.")]
  public async Task It_should_not_do_anything_when_the_language_could_not_be_found()
  {
    Character character = new CharacterBuilder(_world).Build();
    _characterRepository.Setup(x => x.LoadAsync(character.Id, _cancellationToken)).ReturnsAsync(character);

    CharacterLanguagePayload payload = new();
    RemoveCharacterLanguageCommand command = new(character.EntityId, Guid.Empty);
    command.Contextualize(_world);

    CharacterModel model = new();
    _characterQuerier.Setup(x => x.ReadAsync(character, _cancellationToken)).ReturnsAsync(model);

    CharacterModel? result = await _handler.Handle(command, _cancellationToken);
    Assert.NotNull(result);
    Assert.Same(model, result);

    _permissionService.Verify(x => x.EnsureCanUpdateAsync(command, character.GetMetadata(), _cancellationToken), Times.Once);

    _sender.Verify(x => x.Send(It.Is<SaveCharacterCommand>(y => y.Character.Equals(character)), _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should remove an existing character language.")]
  public async Task It_should_set_an_existing_character_language()
  {
    Character character = new CharacterBuilder(_world).Build();
    _characterRepository.Setup(x => x.LoadAsync(character.Id, _cancellationToken)).ReturnsAsync(character);

    Language language = new(_world.Id, new Name("Orrinique"), _world.OwnerId);
    character.SetLanguage(language, notes: null, _world.OwnerId);
    Assert.NotEmpty(character.Languages);

    RemoveCharacterLanguageCommand command = new(character.EntityId, language.EntityId);
    command.Contextualize(_world);

    CharacterModel model = new();
    _characterQuerier.Setup(x => x.ReadAsync(character, _cancellationToken)).ReturnsAsync(model);

    CharacterModel? result = await _handler.Handle(command, _cancellationToken);
    Assert.NotNull(result);
    Assert.Same(model, result);

    Assert.Empty(character.Languages);
    Assert.Contains(character.Changes, change => change is Character.LanguageRemovedEvent e && e.LanguageId == language.Id);

    _permissionService.Verify(x => x.EnsureCanUpdateAsync(command, character.GetMetadata(), _cancellationToken), Times.Once);

    _sender.Verify(x => x.Send(It.Is<SaveCharacterCommand>(y => y.Character.Equals(character)), _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should return null when the character could not be found.")]
  public async Task It_should_return_null_when_the_character_could_not_be_found()
  {
    RemoveCharacterLanguageCommand command = new(Guid.Empty, Guid.Empty);
    command.Contextualize(_world);

    CharacterModel? character = await _handler.Handle(command, _cancellationToken);
    Assert.Null(character);
  }
}
