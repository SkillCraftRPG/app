using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SkillCraft.Contracts.Characters;
using SkillCraft.Domain;
using SkillCraft.EntityFrameworkCore.Entities;

namespace SkillCraft.EntityFrameworkCore.Configurations;

internal class CharacterBonusConfiguration : IEntityTypeConfiguration<CharacterBonusEntity>
{
  public void Configure(EntityTypeBuilder<CharacterBonusEntity> builder)
  {
    builder.ToTable(SkillCraftDb.CharacterBonuses.Table.Table ?? string.Empty, SkillCraftDb.CharacterBonuses.Table.Schema);
    builder.HasKey(x => x.CharacterBonusId);

    builder.HasIndex(x => new { x.CharacterId, x.Id }).IsUnique();
    builder.HasIndex(x => x.Category);
    builder.HasIndex(x => x.Target);
    builder.HasIndex(x => x.Value);
    builder.HasIndex(x => x.IsTemporary);
    builder.HasIndex(x => x.Precision);

    builder.Property(x => x.Category).HasMaxLength(byte.MaxValue).HasConversion(new EnumToStringConverter<BonusCategory>());
    builder.Property(x => x.Target).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.Precision).HasMaxLength(Name.MaximumLength);

    builder.HasOne(x => x.Character).WithMany(x => x.Bonuses)
      .HasPrincipalKey(x => x.CharacterId).HasForeignKey(x => x.CharacterId)
      .OnDelete(DeleteBehavior.Cascade);
  }
}
