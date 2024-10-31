using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SkillCraft.Contracts;
using SkillCraft.Domain;
using SkillCraft.EntityFrameworkCore.Entities;

namespace SkillCraft.EntityFrameworkCore.Configurations;

internal class InventoryConfiguration : IEntityTypeConfiguration<InventoryEntity>
{
  public void Configure(EntityTypeBuilder<InventoryEntity> builder)
  {
    builder.ToTable(SkillCraftDb.Inventory.Table.Table ?? string.Empty, SkillCraftDb.Inventory.Table.Schema);
    builder.HasKey(x => x.InventoryId);

    builder.HasIndex(x => x.Id).IsUnique();
    builder.HasIndex(x => x.ContainingItemId);

    builder.Property(x => x.Skill).HasMaxLength(byte.MaxValue).HasConversion(new EnumToStringConverter<Skill>());
    builder.Property(x => x.NameOverride).HasMaxLength(Name.MaximumLength);
    builder.Property(x => x.DescriptionOverride).HasMaxLength(Description.MaximumLength);

    builder.HasOne(x => x.Character).WithMany(x => x.Inventory)
      .HasPrincipalKey(x => x.CharacterId).HasForeignKey(x => x.CharacterId)
      .OnDelete(DeleteBehavior.Cascade);
    builder.HasOne(x => x.Item).WithMany(x => x.Inventory)
      .HasPrincipalKey(x => x.ItemId).HasForeignKey(x => x.ItemId)
      .OnDelete(DeleteBehavior.Restrict);
  }
}
