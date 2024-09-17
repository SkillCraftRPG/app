using SkillCraft.Domain;
using SkillCraft.Domain.Castes;
using SkillCraft.Infrastructure.Converters;

namespace SkillCraft.Infrastructure.Serialization;

[Trait(Traits.Category, Categories.Unit)]
public class CasteTraitTests
{
  private readonly JsonSerializerOptions _options = new();

  public CasteTraitTests()
  {
    _options.Converters.Add(new DescriptionConverter());
    _options.Converters.Add(new NameConverter());
    _options.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
  }

  [Fact(DisplayName = "It should deserialize correctly.")]
  public void It_should_deserialize_correctly()
  {
    Name name = new("Vagabond");
    Description description = new("Aucun village ni aucune ville n’est un meilleur domicile pour le personnage que la route. Qu’il soit nomade par choix ou par obligation, ses tests de Survie afin de trouver de l’eau, de la nourriture ou un abri se voient conférer l’avantage lorsqu’il se trouve à proximité d’une route maintenue.");
    string json = $@"{{""Name"":""{name}"",""Description"":""{description}""}}";
    Trait? trait = JsonSerializer.Deserialize<Trait>(json, _options);
    Assert.NotNull(trait);
    Assert.Equal(name, trait.Name);
    Assert.Equal(description, trait.Description);
  }

  [Fact(DisplayName = "It should handle null deserialization correctly.")]
  public void It_should_handle_null_deserialization_correctly()
  {
    string json = "null";
    Trait? trait = JsonSerializer.Deserialize<Trait>(json, _options);
    Assert.Null(trait);
  }

  [Fact(DisplayName = "It should serialize correctly.")]
  public void It_should_serialize_correctly()
  {
    Trait trait = new(new Name("Vagabond"), new Description("Aucun village ni aucune ville n’est un meilleur domicile pour le personnage que la route. Qu’il soit nomade par choix ou par obligation, ses tests de Survie afin de trouver de l’eau, de la nourriture ou un abri se voient conférer l’avantage lorsqu’il se trouve à proximité d’une route maintenue."));
    string json = JsonSerializer.Serialize(trait, _options);
    Assert.Equal($@"{{""Name"":""{trait.Name}"",""Description"":""{trait.Description}""}}", json);
  }
}
