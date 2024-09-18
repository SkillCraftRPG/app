using SkillCraft.Contracts.Lineages;
using SkillCraft.Domain;
using SkillCraft.Domain.Lineages;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Application.Lineages;

internal static class LineageExtensions
{
  private const EntityType Type = EntityType.Lineage;

  public static EntityMetadata GetMetadata(this Lineage lineage)
  {
    long size = 4 /* ParentId */ + lineage.Name.Size + (lineage.Description?.Size ?? 0)
      + 32 /* Attributes */ + lineage.Traits.Values.Sum(GetSize)
      + lineage.Languages.Size + lineage.Names.Size
      + 24 /* Speeds */ + GetSize(lineage.Size) + lineage.Weight.Size + 16 /* Ages */;
    return new EntityMetadata(lineage.WorldId, new EntityKey(Type, lineage.Id.ToGuid()), size);
  }
  private static long GetSize(Trait trait) => trait.Name.Size + (trait.Description?.Size ?? 0);
  private static long GetSize(Size size) => 4 + (size.Roll?.Size ?? 0);

  public static EntityMetadata GetMetadata(this LineageModel lineage)
  {
    long size = 4 /* ParentId */ + lineage.Name.Length + (lineage.Description?.Length ?? 0)
      + 32 /* Attributes */ + lineage.Traits.Sum(GetSize)
      + GetSize(lineage.Languages) + GetSize(lineage.Names)
      + 24 /* Speeds */ + GetSize(lineage.Size) + GetSize(lineage.Weight) + 16 /* Ages */;
    return new EntityMetadata(new WorldId(lineage.World.Id), new EntityKey(Type, lineage.Id), size);
  }
  private static long GetSize(TraitModel trait) => trait.Name.Length + (trait.Description?.Length ?? 0);
  private static long GetSize(LanguagesModel languages) => (languages.Items.Count * 4) + 4 + (languages.Text?.Length ?? 0);
  private static long GetSize(NamesModel names) => (names.Text?.Length ?? 0)
    + names.Family.Sum(x => x.Length) + names.Female.Sum(x => x.Length) + names.Male.Sum(x => x.Length) + names.Unisex.Sum(x => x.Length)
    + names.Custom.Sum(GetSize);
  private static long GetSize(NameCategory category) => category.Key.Length + category.Values.Sum(x => x.Length);
  private static long GetSize(SizeModel size) => 4 + (size.Roll?.Length ?? 0);
  private static long GetSize(WeightModel weight) => (weight.Starved?.Length ?? 0)
    + (weight.Skinny?.Length ?? 0) + (weight.Normal?.Length ?? 0)
    + (weight.Overweight?.Length ?? 0) + (weight.Obese?.Length ?? 0);
}
