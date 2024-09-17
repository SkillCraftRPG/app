using SkillCraft.Domain.Lineages;

namespace SkillCraft.Infrastructure.Serialization;

[Trait(Traits.Category, Categories.Unit)]
public class NamesTests
{
  private readonly JsonSerializerOptions _options = new();

  public NamesTests()
  {
    _options.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
  }

  [Fact(DisplayName = "It should deserialize correctly.")]
  public void It_should_deserialize_correctly()
  {
    string text = "les Sophithéons portent les mêmes prénoms que les Orrins. […]";
    string json = $@"{{""Text"":""{text}"",""Family"":[""Aetos"",""Condos""],""Female"":[""Alexandra"",""Ariadnh""],""Male"":[""Amphimedes"",""Demetrios""],""Unisex"":[""Phoenix""],""Custom"":{{""Surnoms"":[""de la Rivière"",""du Ponton""]}}}}";
    Names? names = JsonSerializer.Deserialize<Names>(json, _options);
    Assert.NotNull(names);
    Assert.Equal(text, names.Text);
    Assert.Equal(["Aetos", "Condos"], names.Family);
    Assert.Equal(["Alexandra", "Ariadnh"], names.Female);
    Assert.Equal(["Amphimedes", "Demetrios"], names.Male);
    Assert.Equal(["Phoenix"], names.Unisex);
    KeyValuePair<string, IReadOnlyCollection<string>> custom = Assert.Single(names.Custom);
    Assert.Equal("Surnoms", custom.Key);
    Assert.Equal(["de la Rivière", "du Ponton"], custom.Value);
  }

  [Fact(DisplayName = "It should handle null deserialization correctly.")]
  public void It_should_handle_null_deserialization_correctly()
  {
    string json = "null";
    Names? names = JsonSerializer.Deserialize<Names>(json, _options);
    Assert.Null(names);
  }

  [Fact(DisplayName = "It should serialize correctly.")]
  public void It_should_serialize_correctly()
  {
    Names names = new(
      "les Sophithéons portent les mêmes prénoms que les Orrins. […]",
      ["Aetos", "Condos"],
      ["Alexandra", "Ariadnh"],
      ["Amphimedes", "Demetrios"],
      ["Phoenix"],
      new Dictionary<string, IReadOnlyCollection<string>> { ["Surnoms"] = ["de la Rivière", "du Ponton"] });
    string json = JsonSerializer.Serialize(names, _options);
    Assert.Equal($@"{{""Text"":""{names.Text}"",""Family"":[""Aetos"",""Condos""],""Female"":[""Alexandra"",""Ariadnh""],""Male"":[""Amphimedes"",""Demetrios""],""Unisex"":[""Phoenix""],""Custom"":{{""Surnoms"":[""de la Rivière"",""du Ponton""]}}}}", json);
  }
}
