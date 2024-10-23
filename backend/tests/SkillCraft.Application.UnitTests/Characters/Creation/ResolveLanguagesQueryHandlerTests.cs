using Moq;
using SkillCraft.Application.Characters.Commands;
using SkillCraft.Application.Languages;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Characters;
using SkillCraft.Domain;
using SkillCraft.Domain.Languages;
using SkillCraft.Domain.Lineages;

namespace SkillCraft.Application.Characters.Creation;

[Trait(Traits.Category, Categories.Unit)]
public class ResolveLanguagesQueryHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<ILanguageRepository> _languageRepository = new();
  private readonly Mock<IPermissionService> _permissionService = new();

  private readonly ResolveLanguagesQueryHandler _handler;

  private readonly WorldMock _world = new();
  private readonly Lineage _species;
  private readonly Lineage _nation;
  private readonly Language _language1;
  private readonly Language _language2;
  private readonly CreateCharacterCommand _activity = new(new CreateCharacterPayload());

  public ResolveLanguagesQueryHandlerTests()
  {
    _handler = new(_languageRepository.Object, _permissionService.Object);

    _species = new(_world.Id, parent: null, new Name("Species"), _world.OwnerId)
    {
      Languages = new(ids: [], extra: 1, text: null)
    };
    _nation = new(_world.Id, _species, new Name("Nation"), _world.OwnerId)
    {
      Languages = new(ids: [], extra: 1, text: null)
    };
    _language1 = new(_world.Id, new Name("Orrinique"), _world.OwnerId);
    _language2 = new(_world.Id, new Name("Celfique"), _world.OwnerId);

    _activity.Contextualize(_world);
  }

  [Fact(DisplayName = "It should return prematurely when there is no ID.")]
  public async Task It_should_return_prematurely_when_there_is_no_Id()
  {
    ResolveLanguagesQuery query = new(_activity, _nation, _species, Ids: []);

    IReadOnlyCollection<Language> languages = await _handler.Handle(query, _cancellationToken);
    Assert.Empty(languages);

    _permissionService.Verify(x => x.EnsureCanPreviewAsync(It.IsAny<Activity>(), It.IsAny<EntityType>(), It.IsAny<CancellationToken>()), Times.Never);
  }

  [Fact(DisplayName = "It should return the found languages.")]
  public async Task It_should_return_the_found_languages()
  {
    ResolveLanguagesQuery query = new(_activity, _nation, _species, [_language1.EntityId, _language2.EntityId]);

    IEnumerable<LanguageId> languageIds = query.Ids.Distinct().Select(id => new LanguageId(_world.Id, id));
    _languageRepository.Setup(x => x.LoadAsync(languageIds, _cancellationToken)).ReturnsAsync([_language1, _language2]);

    IReadOnlyCollection<Language> languages = await _handler.Handle(query, _cancellationToken);
    Assert.Equal(2, languages.Count);
    Assert.Contains(_language1, languages);
    Assert.Contains(_language2, languages);

    _permissionService.Verify(x => x.EnsureCanPreviewAsync(_activity, EntityType.Language, _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should throw InvalidExtraLanguagesException when the number of extra languages does not match the lineages count.")]
  public async Task It_should_throw_InvalidExtraLanguagesException_when_the_number_of_extra_languages_does_not_match_the_lineages_count()
  {
    ResolveLanguagesQuery query = new(_activity, _nation, _species, [_language1.EntityId]);

    IEnumerable<LanguageId> languageIds = query.Ids.Distinct().Select(id => new LanguageId(_world.Id, id));
    _languageRepository.Setup(x => x.LoadAsync(languageIds, _cancellationToken)).ReturnsAsync([_language1]);

    var exception = await Assert.ThrowsAsync<InvalidExtraLanguagesException>(async () => await _handler.Handle(query, _cancellationToken));
    Assert.Equal(query.Ids, exception.Ids);
    Assert.Equal(_species.Languages.Extra + _nation.Languages.Extra, exception.ExpectedCount);
    Assert.Equal("LanguageIds", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw LanguagesCannotIncludeLineageLanguageException when some lineage languages are specified.")]
  public async Task It_should_throw_LanguagesCannotIncludeLineageLanguageException_when_some_lineage_languages_are_specified()
  {
    _nation.Languages = new([_language1], extra: 0, text: null);

    ResolveLanguagesQuery query = new(_activity, _nation, _species, [_language1.EntityId]);

    IEnumerable<LanguageId> languageIds = query.Ids.Distinct().Select(id => new LanguageId(_world.Id, id));
    _languageRepository.Setup(x => x.LoadAsync(languageIds, _cancellationToken)).ReturnsAsync([_language1]);

    var exception = await Assert.ThrowsAsync<LanguagesCannotIncludeLineageLanguageException>(async () => await _handler.Handle(query, _cancellationToken));
    Assert.Equal(_language1.EntityId, Assert.Single(exception.LanguageIds));
    Assert.Equal("LanguageIds", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw LanguagesNotFoundException when some languages could not be found.")]
  public async Task It_should_throw_LanguagesNotFoundException_when_some_languages_could_not_be_found()
  {
    ResolveLanguagesQuery query = new(_activity, _nation, _species, [_language1.EntityId, _language2.EntityId, Guid.NewGuid(), Guid.Empty]);

    IEnumerable<LanguageId> languageIds = query.Ids.Distinct().Select(id => new LanguageId(_world.Id, id));
    _languageRepository.Setup(x => x.LoadAsync(languageIds, _cancellationToken)).ReturnsAsync([_language1, _language2]);

    var exception = await Assert.ThrowsAsync<LanguagesNotFoundException>(async () => await _handler.Handle(query, _cancellationToken));
    Assert.Equal(query.Ids.Skip(2), exception.LanguageIds);
    Assert.Equal("LanguageIds", exception.PropertyName);
  }
}
