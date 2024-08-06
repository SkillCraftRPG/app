using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SkillCraft.Domain;
using SkillCraft.EntityFrameworkCore.Entities;

namespace SkillCraft.EntityFrameworkCore.Configurations;

internal class StorageDetailConfiguration : IEntityTypeConfiguration<StorageDetailEntity>
{
  public void Configure(EntityTypeBuilder<StorageDetailEntity> builder)
  {
    builder.ToTable(SkillCraftDb.StorageDetails.Table.Table ?? string.Empty, SkillCraftDb.StorageDetails.Table.Schema);
    builder.HasKey(x => x.StorageDetailId);

    builder.HasIndex(x => x.UserId);
    builder.HasIndex(x => x.WorldId);
    builder.HasIndex(x => new { x.EntityType, x.EntityId }).IsUnique();
    builder.HasIndex(x => x.UsedBytes);

    builder.Property(x => x.EntityType).HasMaxLength(-1).HasConversion(new EnumToStringConverter<EntityType>());

    builder.HasOne(x => x.World).WithMany(x => x.StorageDetails)
      .HasPrincipalKey(x => x.WorldId).HasForeignKey(x => x.WorldId)
      .OnDelete(DeleteBehavior.Restrict);
  }
}
