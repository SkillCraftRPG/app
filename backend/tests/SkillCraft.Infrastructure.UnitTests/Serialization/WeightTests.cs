using SkillCraft.Domain;
using SkillCraft.Domain.Lineages;
using SkillCraft.Infrastructure.Converters;

namespace SkillCraft.Infrastructure.Serialization;

[Trait(Traits.Category, Categories.Unit)]
public class WeightTests
{
  private readonly JsonSerializerOptions _options = new();

  public WeightTests()
  {
    _options.Converters.Add(new RollConverter());
    _options.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
  }

  [Fact(DisplayName = "It should deserialize correctly.")]
  public void It_should_deserialize_correctly()
  {
    string json = @"{""Starved"":""11+1d4"",""Skinny"":""15+1d4"",""Normal"":""19+1d6"",""Overweight"":""26+1d6"",""Obese"":""32+1d10""}";
    Weight? weight = JsonSerializer.Deserialize<Weight>(json, _options);
    Assert.NotNull(weight);
    Assert.Equal("11+1d4", weight.Starved?.Value);
    Assert.Equal("15+1d4", weight.Skinny?.Value);
    Assert.Equal("19+1d6", weight.Normal?.Value);
    Assert.Equal("26+1d6", weight.Overweight?.Value);
    Assert.Equal("32+1d10", weight.Obese?.Value);
  }

  [Fact(DisplayName = "It should handle null deserialization correctly.")]
  public void It_should_handle_null_deserialization_correctly()
  {
    string json = "null";
    Weight? weight = JsonSerializer.Deserialize<Weight>(json, _options);
    Assert.Null(weight);
  }

  [Fact(DisplayName = "It should serialize correctly.")]
  public void It_should_serialize_correctly()
  {
    Weight weight = new(
      new Roll("11+1d4"),
      new Roll("15+1d4"),
      new Roll("19+1d6"),
      new Roll("26+1d6"),
      new Roll("32+1d10"));
    string json = JsonSerializer.Serialize(weight, _options);
    Assert.Equal(@"{""Starved"":""11+1d4"",""Skinny"":""15+1d4"",""Normal"":""19+1d6"",""Overweight"":""26+1d6"",""Obese"":""32+1d10""}", json);
  }
}
