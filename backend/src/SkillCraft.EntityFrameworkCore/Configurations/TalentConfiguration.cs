using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SkillCraft.Contracts;
using SkillCraft.Domain;
using SkillCraft.EntityFrameworkCore.Entities;

namespace SkillCraft.EntityFrameworkCore.Configurations;

internal class TalentConfiguration : AggregateConfiguration<TalentEntity>, IEntityTypeConfiguration<TalentEntity>
{
  public override void Configure(EntityTypeBuilder<TalentEntity> builder)
  {
    base.Configure(builder);

    builder.ToTable(SkillCraftDb.Talents.Table.Table ?? string.Empty, SkillCraftDb.Talents.Table.Schema);
    builder.HasKey(x => x.TalentId);

    builder.HasIndex(x => x.Id).IsUnique();
    builder.HasIndex(x => x.Name);
    builder.HasIndex(x => new { x.WorldId, x.Skill });
    builder.HasIndex(x => x.Skill);

    builder.Property(x => x.Name).HasMaxLength(Slug.MaximumLength);
    builder.Property(x => x.Skill).HasMaxLength(byte.MaxValue).HasConversion(new EnumToStringConverter<Skill>());

    builder.HasOne(x => x.World).WithMany(x => x.Talents)
      .HasPrincipalKey(x => x.WorldId).HasForeignKey(x => x.WorldId)
      .OnDelete(DeleteBehavior.Restrict);
    builder.HasOne(x => x.RequiredTalent).WithMany(x => x.RequiringTalents)
      .HasPrincipalKey(x => x.TalentId).HasForeignKey(x => x.RequiredTalentId)
      .OnDelete(DeleteBehavior.Restrict);
  }
}
