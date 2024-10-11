using Logitar.Portal.Contracts.Actors;
using Moq;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Languages;
using SkillCraft.Contracts.Worlds;

namespace SkillCraft.Application.Languages.Queries;

[Trait(Traits.Category, Categories.Unit)]
public class ReadLanguageQueryHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<ILanguageQuerier> _languageQuerier = new();
  private readonly Mock<IPermissionService> _permissionService = new();

  private readonly ReadLanguageQueryHandler _handler;

  private readonly UserMock _user = new();
  private readonly WorldMock _world;
  private readonly LanguageModel _language;

  public ReadLanguageQueryHandlerTests()
  {
    _handler = new(_languageQuerier.Object, _permissionService.Object);

    _world = new(_user);
    WorldModel worldModel = new(new Actor(_user), _world.Slug.Value)
    {
      Id = _world.Id.ToGuid()
    };
    _language = new(worldModel, "Commun")
    {
      Id = Guid.NewGuid()
    };
    _languageQuerier.Setup(x => x.ReadAsync(_world.Id, _language.Id, _cancellationToken)).ReturnsAsync(_language);
  }

  [Fact(DisplayName = "It should return null when no language is found.")]
  public async Task It_should_return_null_when_no_language_is_found()
  {
    ReadLanguageQuery query = new(Guid.Empty);
    query.Contextualize(_world);

    Assert.Null(await _handler.Handle(query, _cancellationToken));

    _permissionService.Verify(x => x.EnsureCanPreviewAsync(query, EntityType.Language, _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should return the language found by ID.")]
  public async Task It_should_return_the_language_found_by_Id()
  {
    ReadLanguageQuery query = new(_language.Id);
    query.Contextualize(_world);

    LanguageModel? language = await _handler.Handle(query, _cancellationToken);
    Assert.Same(_language, language);

    _permissionService.Verify(x => x.EnsureCanPreviewAsync(query, EntityType.Language, _cancellationToken), Times.Once);
  }
}
