using Logitar;
using SkillCraft.Contracts.Lineages;
using SkillCraft.Domain.Lineages;

namespace SkillCraft.Application.Lineages;

internal static class LineageExtensions
{
  public static bool AreEqualTo(this IAges left, IAges right) => left.Adolescent == right.Adolescent
    && left.Adult == right.Adult
    && left.Mature == right.Mature
    && left.Venerable == right.Venerable;

  public static bool AreEqualTo(this IAttributeBonuses left, IAttributeBonuses right) => left.Agility == right.Agility
    && left.Coordination == right.Coordination
    && left.Intellect == right.Intellect
    && left.Presence == right.Presence
    && left.Sensitivity == right.Sensitivity
    && left.Spirit == right.Spirit
    && left.Vigor == right.Vigor
    && left.Extra == right.Extra;

  public static bool AreEqualTo(this IReadOnlyDictionary<Guid, Feature> left, IEnumerable<FeaturePayload> right)
  {
    Assert.Equal(left.Count, right.Count());

    foreach (FeaturePayload feature in right)
    {
      if (feature.Id.HasValue)
      {
        Assert.True(left.ContainsKey(feature.Id.Value));
        Assert.Equal(feature.Name.Trim(), left[feature.Id.Value].Name.Value);
        Assert.Equal(feature.Description?.CleanTrim(), left[feature.Id.Value].Description?.Value);
      }
      else
      {
        Assert.Contains(left, x => x.Value.Name.Value == feature.Name.Trim()
        && x.Value.Description?.Value == feature.Description?.CleanTrim());
      }
    }

    return true;
  }

  public static bool AreEqualTo(this Domain.Lineages.Languages left, LanguagesPayload right)
  {
    if (left.Extra != right.Extra || left.Text != right.Text?.CleanTrim())
    {
      return false;
    }

    Assert.Equal(left.Ids.Count, right.Ids.Count);
    foreach (Guid id in right.Ids)
    {
      Assert.Contains(left.Ids, i => i.ToGuid() == id);
    }

    return true;
  }

  public static bool AreEqualTo(this Names left, NamesModel right)
  {
    if (left.Text != right.Text?.CleanTrim())
    {
      return false;
    }

    foreach (string family in right.Family)
    {
      if (!string.IsNullOrWhiteSpace(family))
      {
        Assert.Contains(family.Trim(), left.Family);
      }
    }
    foreach (string female in right.Female)
    {
      if (!string.IsNullOrWhiteSpace(female))
      {
        Assert.Contains(female.Trim(), left.Female);
      }
    }
    foreach (string male in right.Male)
    {
      if (!string.IsNullOrWhiteSpace(male))
      {
        Assert.Contains(male.Trim(), left.Male);
      }
    }
    foreach (string unisex in right.Unisex)
    {
      if (!string.IsNullOrWhiteSpace(unisex))
      {
        Assert.Contains(unisex.Trim(), left.Unisex);
      }
    }

    foreach (NameCategory category in right.Custom)
    {
      string key = category.Key.Trim();
      Assert.True(left.Custom.ContainsKey(key));
      IReadOnlyCollection<string> values = left.Custom[key];
      foreach (string value in category.Values)
      {
        if (!string.IsNullOrWhiteSpace(value))
        {
          Assert.Contains(value.Trim(), values);
        }
      }
    }

    return true;
  }

  public static bool AreEqualTo(this ISpeeds left, ISpeeds right) => left.Walk == right.Walk
    && left.Climb == right.Climb
    && left.Swim == right.Swim
    && left.Fly == right.Fly
    && left.Hover == right.Hover
    && left.Burrow == right.Burrow;

  public static bool IsEqualTo(this Size left, SizeModel right) => left.Category == right.Category
    && left.Roll?.Value == right.Roll?.CleanTrim();

  public static bool IsEqualTo(this Weight left, WeightModel right) => left.Starved?.Value == right.Starved?.CleanTrim()
    && left.Skinny?.Value == right.Skinny?.CleanTrim()
    && left.Normal?.Value == right.Normal?.CleanTrim()
    && left.Overweight?.Value == right.Overweight?.CleanTrim()
    && left.Obese?.Value == right.Obese?.CleanTrim();
}
