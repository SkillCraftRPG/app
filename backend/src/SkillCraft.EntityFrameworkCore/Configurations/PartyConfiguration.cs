using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkillCraft.Domain;
using SkillCraft.EntityFrameworkCore.Entities;

namespace SkillCraft.EntityFrameworkCore.Configurations;

internal class PartyConfiguration : AggregateConfiguration<PartyEntity>, IEntityTypeConfiguration<PartyEntity>
{
  public override void Configure(EntityTypeBuilder<PartyEntity> builder)
  {
    base.Configure(builder);

    builder.ToTable(SkillCraftDb.Parties.Table.Table ?? string.Empty, SkillCraftDb.Parties.Table.Schema);
    builder.HasKey(x => x.PartyId);

    builder.HasIndex(x => new { x.WorldId, x.Id }).IsUnique();
    builder.HasIndex(x => x.Name);

    builder.Property(x => x.Name).HasMaxLength(Slug.MaximumLength);

    builder.HasOne(x => x.World).WithMany(x => x.Parties)
      .HasPrincipalKey(x => x.WorldId).HasForeignKey(x => x.WorldId)
      .OnDelete(DeleteBehavior.Restrict);
  }
}
