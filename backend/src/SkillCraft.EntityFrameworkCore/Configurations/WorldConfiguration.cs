using Logitar.Identity.Domain.Shared;
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
    builder.HasIndex(x => x.UniqueSlug);
    builder.HasIndex(x => x.UniqueSlugNormalized).IsUnique();
    builder.HasIndex(x => x.DisplayName);

    builder.Property(x => x.UniqueSlug).HasMaxLength(SlugUnit.MaximumLength);
    builder.Property(x => x.UniqueSlugNormalized).HasMaxLength(SlugUnit.MaximumLength);
    builder.Property(x => x.DisplayName).HasMaxLength(DisplayNameUnit.MaximumLength);
  }
}
