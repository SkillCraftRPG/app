using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SkillCraft.Domain;
using SkillCraft.EntityFrameworkCore.Entities;
using Attribute = SkillCraft.Contracts.Attribute;

namespace SkillCraft.EntityFrameworkCore.Configurations;

internal class NatureConfiguration : AggregateConfiguration<NatureEntity>, IEntityTypeConfiguration<NatureEntity>
{
  public override void Configure(EntityTypeBuilder<NatureEntity> builder)
  {
    base.Configure(builder);

    builder.ToTable(SkillCraftDb.Natures.Table.Table ?? string.Empty, SkillCraftDb.Natures.Table.Schema);
    builder.HasKey(x => x.NatureId);

    builder.HasIndex(x => new { x.WorldId, x.Id }).IsUnique();
    builder.HasIndex(x => x.Name);
    builder.HasIndex(x => x.Attribute);

    builder.Property(x => x.Name).HasMaxLength(Name.MaximumLength);
    builder.Property(x => x.Attribute).HasMaxLength(byte.MaxValue).HasConversion(new EnumToStringConverter<Attribute>());

    builder.HasOne(x => x.Gift).WithMany(x => x.Natures)
      .HasPrincipalKey(x => x.CustomizationId).HasForeignKey(x => x.GiftId)
      .OnDelete(DeleteBehavior.Restrict);
    builder.HasOne(x => x.World).WithMany(x => x.Natures)
      .HasPrincipalKey(x => x.WorldId).HasForeignKey(x => x.WorldId)
      .OnDelete(DeleteBehavior.Restrict);
  }
}
