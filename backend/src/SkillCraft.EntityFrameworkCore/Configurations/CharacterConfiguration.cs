using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SkillCraft.Domain;
using SkillCraft.Domain.Characters;
using SkillCraft.EntityFrameworkCore.Entities;
using Attribute = SkillCraft.Contracts.Attribute;

namespace SkillCraft.EntityFrameworkCore.Configurations;

internal class CharacterConfiguration : AggregateConfiguration<CharacterEntity>, IEntityTypeConfiguration<CharacterEntity>
{
  public override void Configure(EntityTypeBuilder<CharacterEntity> builder)
  {
    base.Configure(builder);

    builder.ToTable(SkillCraftDb.Characters.Table.Table ?? string.Empty, SkillCraftDb.Characters.Table.Schema);
    builder.HasKey(x => x.CharacterId);

    builder.HasIndex(x => new { x.WorldId, x.Id }).IsUnique();
    builder.HasIndex(x => x.Name);
    builder.HasIndex(x => x.PlayerName);

    builder.Property(x => x.Name).HasMaxLength(Name.MaximumLength);
    builder.Property(x => x.PlayerName).HasMaxLength(PlayerName.MaximumLength);
    builder.Property(x => x.BestAttribute).HasMaxLength(byte.MaxValue).HasConversion(new EnumToStringConverter<Attribute>());
    builder.Property(x => x.WorstAttribute).HasMaxLength(byte.MaxValue).HasConversion(new EnumToStringConverter<Attribute>());
    builder.Property(x => x.MandatoryAttributes).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.OptionalAttributes).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.ExtraAttributes).HasMaxLength(byte.MaxValue);

    builder.HasOne(x => x.World).WithMany(x => x.Characters)
      .HasPrincipalKey(x => x.WorldId).HasForeignKey(x => x.WorldId)
      .OnDelete(DeleteBehavior.Restrict);
    builder.HasOne(x => x.Lineage).WithMany(x => x.Characters)
      .HasPrincipalKey(x => x.LineageId).HasForeignKey(x => x.LineageId)
      .OnDelete(DeleteBehavior.Restrict);
    builder.HasOne(x => x.Nature).WithMany(x => x.Characters)
      .HasPrincipalKey(x => x.NatureId).HasForeignKey(x => x.NatureId)
      .OnDelete(DeleteBehavior.Restrict);
    builder.HasMany(x => x.Customizations).WithMany(x => x.Characters)
      .UsingEntity<CharacterCustomizationEntity>(join =>
      {
        join.ToTable(SkillCraftDb.CharacterCustomizations.Table.Table ?? string.Empty, SkillCraftDb.CharacterCustomizations.Table.Schema);
        join.HasKey(x => new { x.CharacterId, x.CustomizationId });
      });
    builder.HasMany(x => x.Aspects).WithMany(x => x.Characters)
      .UsingEntity<CharacterAspectEntity>(join =>
      {
        join.ToTable(SkillCraftDb.CharacterAspects.Table.Table ?? string.Empty, SkillCraftDb.CharacterAspects.Table.Schema);
        join.HasKey(x => new { x.CharacterId, x.AspectId });
      });
    builder.HasOne(x => x.Caste).WithMany(x => x.Characters)
      .HasPrincipalKey(x => x.CasteId).HasForeignKey(x => x.CasteId)
      .OnDelete(DeleteBehavior.Restrict);
    builder.HasOne(x => x.Education).WithMany(x => x.Characters)
      .HasPrincipalKey(x => x.EducationId).HasForeignKey(x => x.EducationId)
      .OnDelete(DeleteBehavior.Restrict);
  }
}
