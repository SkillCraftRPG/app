using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkillCraft.EntityFrameworkCore.Entities;

namespace SkillCraft.EntityFrameworkCore.Configurations;

internal class StorageSummaryConfiguration : AggregateConfiguration<StorageSummaryEntity>, IEntityTypeConfiguration<StorageSummaryEntity>
{
  public override void Configure(EntityTypeBuilder<StorageSummaryEntity> builder)
  {
    base.Configure(builder);

    builder.ToTable(SkillCraftDb.StorageSummaries.Table.Table ?? string.Empty, SkillCraftDb.StorageSummaries.Table.Schema);
    builder.HasKey(x => x.UserId);

    builder.HasIndex(x => x.OwnerId).IsUnique();
    builder.HasIndex(x => x.AllocatedBytes);
    builder.HasIndex(x => x.UsedBytes);
    builder.HasIndex(x => x.AvailableBytes);

    builder.HasOne(x => x.Owner).WithOne(x => x.StorageSummary)
      .HasPrincipalKey<UserEntity>(x => x.UserId).HasForeignKey<StorageSummaryEntity>(x => x.UserId)
      .OnDelete(DeleteBehavior.Restrict);
  }
}
