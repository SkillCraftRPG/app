using Moq;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts.Languages;
using SkillCraft.Contracts.Worlds;
using SkillCraft.Domain;

namespace SkillCraft.Application.Languages.Queries;

[Trait(Traits.Category, Categories.Unit)]
public class ReadLanguageQueryHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<ILanguageQuerier> _languageQuerier = new();
  private readonly Mock<IPermissionService> _permissionService = new();

  private readonly ReadLanguageQueryHandler _handler;

  private readonly LanguageModel _language = new(new WorldModel(), "Commun") { Id = Guid.NewGuid() };

  public ReadLanguageQueryHandlerTests()
  {
    _handler = new(_languageQuerier.Object, _permissionService.Object);

    _languageQuerier.Setup(x => x.ReadAsync(_language.Id, _cancellationToken)).ReturnsAsync(_language);
  }

  [Fact(DisplayName = "It should return null when no language is found.")]
  public async Task It_should_return_null_when_no_language_is_found()
  {
    ReadLanguageQuery query = new(Guid.Empty);
    Assert.Null(await _handler.Handle(query, _cancellationToken));

    _permissionService.Verify(x => x.EnsureCanPreviewAsync(It.IsAny<ReadLanguageQuery>(), It.IsAny<EntityMetadata>(), It.IsAny<CancellationToken>()), Times.Never);
  }

  [Fact(DisplayName = "It should return the language found by ID.")]
  public async Task It_should_return_the_language_found_by_Id()
  {
    ReadLanguageQuery query = new(_language.Id);
    LanguageModel? language = await _handler.Handle(query, _cancellationToken);
    Assert.Same(_language, language);

    _permissionService.Verify(x => x.EnsureCanPreviewAsync(
      query,
      It.Is<EntityMetadata>(y => y.WorldId.ToGuid() == _language.World.Id && y.Key.Type == EntityType.Language && y.Key.Id == _language.Id && y.Size > 0),
      _cancellationToken), Times.Once);
  }
}
