using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkillCraft.Domain;
using SkillCraft.EntityFrameworkCore.Entities;

namespace SkillCraft.EntityFrameworkCore.Configurations;

internal class WorldConfiguration : AggregateConfiguration<WorldEntity>, IEntityTypeConfiguration<WorldEntity>
{
  public override void Configure(EntityTypeBuilder<WorldEntity> builder)
  {
    base.Configure(builder);

    builder.ToTable(SkillCraftDb.Worlds.Table.Table ?? string.Empty, SkillCraftDb.Worlds.Table.Schema);
    builder.HasKey(x => x.WorldId);

    builder.HasIndex(x => x.Id).IsUnique();
    builder.HasIndex(x => x.OwnerId);
    builder.HasIndex(x => x.Slug);
    builder.HasIndex(x => x.SlugNormalized).IsUnique();
    builder.HasIndex(x => x.Name);

    builder.Property(x => x.Slug).HasMaxLength(Slug.MaximumLength);
    builder.Property(x => x.SlugNormalized).HasMaxLength(Slug.MaximumLength);
    builder.Property(x => x.Name).HasMaxLength(Slug.MaximumLength);

    builder.HasOne(x => x.Owner).WithMany(x => x.Worlds)
      .HasPrincipalKey(x => x.UserId).HasForeignKey(x => x.UserId)
      .OnDelete(DeleteBehavior.Restrict);
  }
}
