using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SkillCraft.Contracts.Items;
using SkillCraft.Domain;
using SkillCraft.EntityFrameworkCore.Entities;

namespace SkillCraft.EntityFrameworkCore.Configurations;

internal class ItemConfiguration : AggregateConfiguration<ItemEntity>, IEntityTypeConfiguration<ItemEntity>
{
  public override void Configure(EntityTypeBuilder<ItemEntity> builder)
  {
    base.Configure(builder);

    builder.ToTable(SkillCraftDb.Items.Table.Table ?? string.Empty, SkillCraftDb.Items.Table.Schema);
    builder.HasKey(x => x.ItemId);

    builder.HasIndex(x => new { x.WorldId, x.Id }).IsUnique();
    builder.HasIndex(x => x.Name);
    builder.HasIndex(x => x.Value);
    builder.HasIndex(x => x.Weight);
    builder.HasIndex(x => x.IsAttunementRequired);
    builder.HasIndex(x => x.Category);

    builder.Property(x => x.Name).HasMaxLength(Slug.MaximumLength);
    builder.Property(x => x.Category).HasMaxLength(byte.MaxValue).HasConversion(new EnumToStringConverter<ItemCategory>());

    builder.HasOne(x => x.World).WithMany(x => x.Items)
      .HasPrincipalKey(x => x.WorldId).HasForeignKey(x => x.WorldId)
      .OnDelete(DeleteBehavior.Restrict);
  }
}
