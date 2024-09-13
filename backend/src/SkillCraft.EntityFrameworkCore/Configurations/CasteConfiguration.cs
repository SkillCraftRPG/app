using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SkillCraft.Contracts;
using SkillCraft.Domain;
using SkillCraft.EntityFrameworkCore.Entities;

namespace SkillCraft.EntityFrameworkCore.Configurations;

internal class CasteConfiguration : AggregateConfiguration<CasteEntity>, IEntityTypeConfiguration<CasteEntity>
{
  public override void Configure(EntityTypeBuilder<CasteEntity> builder)
  {
    base.Configure(builder);

    builder.ToTable(SkillCraftDb.Castes.Table.Table ?? string.Empty, SkillCraftDb.Castes.Table.Schema);
    builder.HasKey(x => x.CasteId);

    builder.HasIndex(x => x.Id).IsUnique();
    builder.HasIndex(x => x.Name);

    builder.Property(x => x.Name).HasMaxLength(Slug.MaximumLength);
    builder.Property(x => x.Skill).HasMaxLength(byte.MaxValue).HasConversion(new EnumToStringConverter<Skill>());
    builder.Property(x => x.WealthRoll).HasMaxLength(Roll.MaximumLength);
    // TODO(fpion): Traits

    builder.HasOne(x => x.World).WithMany(x => x.Castes)
      .HasPrincipalKey(x => x.WorldId).HasForeignKey(x => x.WorldId)
      .OnDelete(DeleteBehavior.Restrict);
  }
}
