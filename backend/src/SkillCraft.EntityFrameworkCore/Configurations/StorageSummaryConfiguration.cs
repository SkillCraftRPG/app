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
    builder.HasKey(x => x.StorageSummaryId);

    builder.HasIndex(x => x.UserId).IsUnique();
    builder.HasIndex(x => x.AllocatedBytes);
    builder.HasIndex(x => x.UsedBytes);
    builder.HasIndex(x => x.AvailableBytes);
  }
}
