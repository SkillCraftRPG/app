using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SkillCraft.Contracts;
using SkillCraft.Domain;
using SkillCraft.EntityFrameworkCore.Entities;
using Attribute = SkillCraft.Contracts.Attribute;

namespace SkillCraft.EntityFrameworkCore.Configurations;

internal class AspectConfiguration : AggregateConfiguration<AspectEntity>, IEntityTypeConfiguration<AspectEntity>
{
  public override void Configure(EntityTypeBuilder<AspectEntity> builder)
  {
    base.Configure(builder);

    builder.ToTable(SkillCraftDb.Aspects.Table.Table ?? string.Empty, SkillCraftDb.Aspects.Table.Schema);
    builder.HasKey(x => x.AspectId);

    builder.HasIndex(x => x.Id).IsUnique();
    builder.HasIndex(x => x.Name);

    builder.Property(x => x.Name).HasMaxLength(Slug.MaximumLength);
    builder.Property(x => x.MandatoryAttribute1).HasMaxLength(byte.MaxValue).HasConversion(new EnumToStringConverter<Attribute>());
    builder.Property(x => x.MandatoryAttribute2).HasMaxLength(byte.MaxValue).HasConversion(new EnumToStringConverter<Attribute>());
    builder.Property(x => x.OptionalAttribute1).HasMaxLength(byte.MaxValue).HasConversion(new EnumToStringConverter<Attribute>());
    builder.Property(x => x.OptionalAttribute2).HasMaxLength(byte.MaxValue).HasConversion(new EnumToStringConverter<Attribute>());
    builder.Property(x => x.DiscountedSkill1).HasMaxLength(byte.MaxValue).HasConversion(new EnumToStringConverter<Skill>());
    builder.Property(x => x.DiscountedSkill2).HasMaxLength(byte.MaxValue).HasConversion(new EnumToStringConverter<Skill>());

    builder.HasOne(x => x.World).WithMany(x => x.Aspects)
      .HasPrincipalKey(x => x.WorldId).HasForeignKey(x => x.WorldId)
      .OnDelete(DeleteBehavior.Restrict);
  }
}
