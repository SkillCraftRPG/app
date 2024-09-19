using SkillCraft.Domain;
using SkillCraft.Domain.Lineages;
using SkillCraft.Infrastructure.Converters;

namespace SkillCraft.Infrastructure.Serialization;

[Trait(Traits.Category, Categories.Unit)]
public class FeatureTests
{
  private readonly JsonSerializerOptions _options = new();

  public FeatureTests()
  {
    _options.Converters.Add(new DescriptionConverter());
    _options.Converters.Add(new NameConverter());
    _options.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
  }

  [Fact(DisplayName = "It should deserialize correctly.")]
  public void It_should_deserialize_correctly()
  {
    Name name = new("Lien psychique");
    Description description = new("Par une action, le personnage peut créer un lien télépathique avec une créature qu’il peut voir et située à 18 mètres (12 cases) ou moins de sa position. […]");
    string json = $@"{{""Name"":""{name}"",""Description"":""{description}""}}";
    Feature? feature = JsonSerializer.Deserialize<Feature>(json, _options);
    Assert.NotNull(feature);
    Assert.Equal(name, feature.Name);
    Assert.Equal(description, feature.Description);
  }

  [Fact(DisplayName = "It should handle null deserialization correctly.")]
  public void It_should_handle_null_deserialization_correctly()
  {
    string json = "null";
    Feature? feature = JsonSerializer.Deserialize<Feature>(json, _options);
    Assert.Null(feature);
  }

  [Fact(DisplayName = "It should serialize correctly.")]
  public void It_should_serialize_correctly()
  {
    Feature feature = new(new Name("Lien psychique"), new Description("Par une action, le personnage peut créer un lien télépathique avec une créature qu’il peut voir et située à 18 mètres (12 cases) ou moins de sa position. […]"));
    string json = JsonSerializer.Serialize(feature, _options);
    Assert.Equal($@"{{""Name"":""{feature.Name}"",""Description"":""{feature.Description}""}}", json);
  }
}
