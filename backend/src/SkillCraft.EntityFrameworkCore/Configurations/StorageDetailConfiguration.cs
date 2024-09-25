using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SkillCraft.Contracts;
using SkillCraft.EntityFrameworkCore.Entities;

namespace SkillCraft.EntityFrameworkCore.Configurations;

internal class StorageDetailConfiguration : IEntityTypeConfiguration<StorageDetailEntity>
{
  public void Configure(EntityTypeBuilder<StorageDetailEntity> builder)
  {
    builder.ToTable(SkillCraftDb.StorageDetails.Table.Table ?? string.Empty, SkillCraftDb.StorageDetails.Table.Schema);
    builder.HasKey(x => x.StorageDetailId);

    builder.HasIndex(x => x.OwnerId);
    builder.HasIndex(x => new { x.EntityType, x.EntityId }).IsUnique(); // TODO(fpion): in the same world
    builder.HasIndex(x => x.Size);

    builder.Property(x => x.EntityType).HasMaxLength(byte.MaxValue).HasConversion(new EnumToStringConverter<EntityType>());

    builder.HasOne(x => x.Owner).WithMany(x => x.StorageDetails)
      .HasPrincipalKey(x => x.UserId).HasForeignKey(x => x.UserId)
      .OnDelete(DeleteBehavior.Restrict);
    builder.HasOne(x => x.World).WithMany(x => x.StorageDetails)
      .HasPrincipalKey(x => x.WorldId).HasForeignKey(x => x.WorldId)
      .OnDelete(DeleteBehavior.Restrict);
  }
}
