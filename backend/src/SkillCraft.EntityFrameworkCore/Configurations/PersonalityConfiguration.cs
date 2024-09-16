using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SkillCraft.Domain;
using SkillCraft.EntityFrameworkCore.Entities;
using Attribute = SkillCraft.Contracts.Attribute;

namespace SkillCraft.EntityFrameworkCore.Configurations;

internal class PersonalityConfiguration : AggregateConfiguration<PersonalityEntity>, IEntityTypeConfiguration<PersonalityEntity>
{
  public override void Configure(EntityTypeBuilder<PersonalityEntity> builder)
  {
    base.Configure(builder);

    builder.ToTable(SkillCraftDb.Personalities.Table.Table ?? string.Empty, SkillCraftDb.Personalities.Table.Schema);
    builder.HasKey(x => x.PersonalityId);

    builder.HasIndex(x => x.Id).IsUnique();
    builder.HasIndex(x => x.Name);

    builder.Property(x => x.Name).HasMaxLength(Slug.MaximumLength);
    builder.Property(x => x.Attribute).HasMaxLength(byte.MaxValue).HasConversion(new EnumToStringConverter<Attribute>());

    builder.HasOne(x => x.Gift).WithMany(x => x.Personalities)
      .HasPrincipalKey(x => x.CustomizationId).HasForeignKey(x => x.GiftId)
      .OnDelete(DeleteBehavior.Restrict);
    builder.HasOne(x => x.World).WithMany(x => x.Personalities)
      .HasPrincipalKey(x => x.WorldId).HasForeignKey(x => x.WorldId)
      .OnDelete(DeleteBehavior.Restrict);
  }
}
