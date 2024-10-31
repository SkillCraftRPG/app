using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkillCraft.Domain;
using SkillCraft.EntityFrameworkCore.Entities;

namespace SkillCraft.EntityFrameworkCore.Configurations;

internal class CharacterTalentConfiguration : IEntityTypeConfiguration<CharacterTalentEntity>
{
  public void Configure(EntityTypeBuilder<CharacterTalentEntity> builder)
  {
    builder.ToTable(SkillCraftDb.CharacterTalents.Table.Table ?? string.Empty, SkillCraftDb.CharacterTalents.Table.Schema);
    builder.HasKey(x => x.CharacterTalentId);

    builder.HasIndex(x => x.Id).IsUnique();

    builder.Property(x => x.Precision).HasMaxLength(Name.MaximumLength);

    builder.HasOne(x => x.Character).WithMany(x => x.Talents)
      .HasPrincipalKey(x => x.CharacterId).HasForeignKey(x => x.CharacterId)
      .OnDelete(DeleteBehavior.Cascade);
    builder.HasOne(x => x.Talent).WithMany(x => x.Characters)
      .HasPrincipalKey(x => x.TalentId).HasForeignKey(x => x.TalentId)
      .OnDelete(DeleteBehavior.Restrict);
  }
}
