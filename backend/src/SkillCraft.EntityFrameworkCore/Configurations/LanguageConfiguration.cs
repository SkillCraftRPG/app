using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkillCraft.Domain;
using SkillCraft.Domain.Languages;
using SkillCraft.EntityFrameworkCore.Entities;

namespace SkillCraft.EntityFrameworkCore.Configurations;

internal class LanguageConfiguration : AggregateConfiguration<LanguageEntity>, IEntityTypeConfiguration<LanguageEntity>
{
  public override void Configure(EntityTypeBuilder<LanguageEntity> builder)
  {
    base.Configure(builder);

    builder.ToTable(SkillCraftDb.Languages.Table.Table ?? string.Empty, SkillCraftDb.Languages.Table.Schema);
    builder.HasKey(x => x.LanguageId);

    builder.HasIndex(x => new { x.WorldId, x.Id }).IsUnique();
    builder.HasIndex(x => x.Name);
    builder.HasIndex(x => x.Script);
    builder.HasIndex(x => x.TypicalSpeakers);

    builder.Property(x => x.Name).HasMaxLength(Name.MaximumLength);
    builder.Property(x => x.Script).HasMaxLength(Script.MaximumLength);
    builder.Property(x => x.TypicalSpeakers).HasMaxLength(TypicalSpeakers.MaximumLength);

    builder.HasOne(x => x.World).WithMany(x => x.Languages)
      .HasPrincipalKey(x => x.WorldId).HasForeignKey(x => x.WorldId)
      .OnDelete(DeleteBehavior.Restrict);
  }
}
