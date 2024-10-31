using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkillCraft.EntityFrameworkCore.Entities;

namespace SkillCraft.EntityFrameworkCore.Configurations;

internal class CharacterLanguageConfiguration : IEntityTypeConfiguration<CharacterLanguageEntity>
{
  public void Configure(EntityTypeBuilder<CharacterLanguageEntity> builder)
  {
    builder.ToTable(SkillCraftDb.CharacterLanguages.Table.Table ?? string.Empty, SkillCraftDb.CharacterLanguages.Table.Schema);
    builder.HasKey(x => new { x.CharacterId, x.LanguageId });

    builder.HasOne(x => x.Character).WithMany(x => x.Languages)
      .HasPrincipalKey(x => x.CharacterId).HasForeignKey(x => x.CharacterId)
      .OnDelete(DeleteBehavior.Cascade);
    builder.HasOne(x => x.Language).WithMany(x => x.Characters)
      .HasPrincipalKey(x => x.LanguageId).HasForeignKey(x => x.LanguageId)
      .OnDelete(DeleteBehavior.Restrict);
  }
}
