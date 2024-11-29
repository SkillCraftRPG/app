using SkillCraft.Domain.Languages;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Domain.Characters;

[Trait(Traits.Category, Categories.Unit)]
public class CharacterLanguageTests
{
  private readonly WorldMock _world = new();
  private readonly Language _language;
  private readonly Character _character;

  public CharacterLanguageTests()
  {
    _language = new(_world.Id, new Name("Languedoïl"), _world.OwnerId);
    _character = new CharacterBuilder(_world).Build();
  }

  [Fact(DisplayName = "RemoveLanguage: it should not do anything when the language was not found.")]
  public void RemoveLanguage_it_should_not_do_anything_when_the_language_was_not_found()
  {
    Assert.Empty(_character.Languages);
    _character.RemoveLanguage(_language.Id, _world.OwnerId);

    Assert.DoesNotContain(_character.Changes, change => change is Character.LanguageRemovedEvent);
  }

  [Fact(DisplayName = "RemoveLanguage: it should remove an existing language.")]
  public void RemoveLanguage_it_should_remove_an_existing_language()
  {
    _character.SetLanguage(_language, notes: null, _world.OwnerId);
    Assert.NotEmpty(_character.Languages);

    _character.RemoveLanguage(_language.Id, _world.OwnerId);
    Assert.Empty(_character.Languages);

    Assert.Contains(_character.Changes, change => change is Character.LanguageRemovedEvent e && e.LanguageId == _language.Id);
  }

  [Fact(DisplayName = "SetLanguage: it should add a new language.")]
  public void SetLanguage_it_should_add_a_new_language()
  {
    _character.SetLanguage(_language, notes: null, _world.OwnerId);
    Assert.Contains(_character.Languages, x => x.Key == _language.Id && x.Value.Notes == null);
  }

  [Fact(DisplayName = "SetLanguage: it should not do anything when the language metadata did not change.")]
  public void SetLanguage_it_should_not_do_anything_when_the_language_metadata_did_not_change()
  {
    _character.SetLanguage(_language, notes: null, _world.OwnerId);
    _character.ClearChanges();

    _character.SetLanguage(_language, notes: null, _world.OwnerId);
    Assert.False(_character.HasChanges);
    Assert.Empty(_character.Changes);
  }

  [Fact(DisplayName = "SetLanguage: it should replace an existing language.")]
  public void SetLanguage_it_should_replace_an_existing_language()
  {
    _character.SetLanguage(_language, notes: null, _world.OwnerId);

    Description notes = new("Lineage Extra Language");
    _character.SetLanguage(_language, notes, _world.OwnerId);
    Assert.Contains(_character.Languages, x => x.Key == _language.Id && x.Value.Notes == notes);
  }

  [Fact(DisplayName = "SetLanguage: it should throw ArgumentException when the language resides in another world.")]
  public void SetLanguage_it_should_throw_ArgumentException_when_the_language_resides_in_another_world()
  {
    UserId userId = UserId.NewId();
    Language language = new(WorldId.NewId(), new Name("Orrinique"), userId);

    var exception = Assert.Throws<ArgumentException>(() => _character.SetLanguage(language, notes: null, userId));
    Assert.StartsWith("The language does not reside in the same world as the character.", exception.Message);
    Assert.Equal("language", exception.ParamName);
  }
}
