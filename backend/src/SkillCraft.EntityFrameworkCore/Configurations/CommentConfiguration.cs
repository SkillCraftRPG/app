using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SkillCraft.Contracts;
using SkillCraft.Domain.Comments;
using SkillCraft.EntityFrameworkCore.Entities;

namespace SkillCraft.EntityFrameworkCore.Configurations;

internal class CommentConfiguration : AggregateConfiguration<CommentEntity>, IEntityTypeConfiguration<CommentEntity>
{
  public override void Configure(EntityTypeBuilder<CommentEntity> builder)
  {
    base.Configure(builder);

    builder.ToTable(SkillCraftDb.Comments.Table.Table ?? string.Empty, SkillCraftDb.Comments.Table.Schema);
    builder.HasKey(x => x.CommentId);

    builder.HasIndex(x => x.Id).IsUnique();
    builder.HasIndex(x => new { x.WorldId, x.EntityType, x.EntityId });

    builder.Property(x => x.EntityType).HasMaxLength(byte.MaxValue).HasConversion(new EnumToStringConverter<EntityType>());
    builder.Property(x => x.Text).HasMaxLength(Text.MaximumLength);

    builder.HasOne(x => x.World).WithMany(x => x.Comments)
      .HasPrincipalKey(x => x.WorldId).HasForeignKey(x => x.WorldId)
      .OnDelete(DeleteBehavior.Restrict);
  }
}
