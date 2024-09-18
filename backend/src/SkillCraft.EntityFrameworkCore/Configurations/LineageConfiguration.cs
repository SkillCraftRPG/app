using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkillCraft.Domain;
using SkillCraft.EntityFrameworkCore.Entities;

namespace SkillCraft.EntityFrameworkCore.Configurations;

internal class LineageConfiguration : AggregateConfiguration<LineageEntity>, IEntityTypeConfiguration<LineageEntity>
{
  public override void Configure(EntityTypeBuilder<LineageEntity> builder)
  {
    base.Configure(builder);

    builder.ToTable(SkillCraftDb.Lineages.Table.Table ?? string.Empty, SkillCraftDb.Lineages.Table.Schema);
    builder.HasKey(x => x.LineageId);

    builder.HasIndex(x => x.Id).IsUnique();
    builder.HasIndex(x => x.Name);

    builder.Property(x => x.Name).HasMaxLength(Slug.MaximumLength);

    builder.HasOne(x => x.World).WithMany(x => x.Lineages)
      .HasPrincipalKey(x => x.WorldId).HasForeignKey(x => x.WorldId)
      .OnDelete(DeleteBehavior.Restrict);
    builder.HasOne(x => x.Species).WithMany(x => x.Nations)
      .HasPrincipalKey(x => x.LineageId).HasForeignKey(x => x.ParentId)
      .OnDelete(DeleteBehavior.Restrict);
  }
}
