using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SkillCraft.Contracts;
using SkillCraft.Domain;
using SkillCraft.Domain.Lineages;
using SkillCraft.EntityFrameworkCore.Entities;

namespace SkillCraft.EntityFrameworkCore.Configurations;

internal class LineageConfiguration : AggregateConfiguration<LineageEntity>, IEntityTypeConfiguration<LineageEntity>
{
  public override void Configure(EntityTypeBuilder<LineageEntity> builder)
  {
    base.Configure(builder);

    builder.ToTable(SkillCraftDb.Lineages.Table.Table ?? string.Empty, SkillCraftDb.Lineages.Table.Schema);
    builder.HasKey(x => x.LineageId);

    builder.HasIndex(x => new { x.WorldId, x.Id }).IsUnique();
    builder.HasIndex(x => x.Name);

    builder.Property(x => x.Name).HasMaxLength(Name.MaximumLength);
    builder.Property(x => x.LanguagesText).HasMaxLength(Languages.MaximumLength);
    builder.Property(x => x.NamesText).HasMaxLength(Names.MaximumLength);
    builder.Property(x => x.SizeCategory).HasMaxLength(byte.MaxValue).HasConversion(new EnumToStringConverter<SizeCategory>());
    builder.Property(x => x.SizeRoll).HasMaxLength(Roll.MaximumLength);
    builder.Property(x => x.StarvedRoll).HasMaxLength(Roll.MaximumLength);
    builder.Property(x => x.SkinnyRoll).HasMaxLength(Roll.MaximumLength);
    builder.Property(x => x.NormalRoll).HasMaxLength(Roll.MaximumLength);
    builder.Property(x => x.OverweightRoll).HasMaxLength(Roll.MaximumLength);
    builder.Property(x => x.ObeseRoll).HasMaxLength(Roll.MaximumLength);

    builder.HasOne(x => x.World).WithMany(x => x.Lineages)
      .HasPrincipalKey(x => x.WorldId).HasForeignKey(x => x.WorldId)
      .OnDelete(DeleteBehavior.Restrict);
    builder.HasOne(x => x.Species).WithMany(x => x.Nations)
      .HasPrincipalKey(x => x.LineageId).HasForeignKey(x => x.ParentId)
      .OnDelete(DeleteBehavior.Restrict);
    builder.HasMany(x => x.Languages).WithMany(x => x.Lineages)
      .UsingEntity<LineageLanguageEntity>(b =>
      {
        b.ToTable(SkillCraftDb.LineageLanguages.Table.Table ?? string.Empty, SkillCraftDb.LineageLanguages.Table.Schema);
        b.HasKey(x => new { x.LineageId, x.LanguageId });
      });
  }
}
