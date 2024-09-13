﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SkillCraft.Contracts;
using SkillCraft.Domain;
using SkillCraft.EntityFrameworkCore.Entities;

namespace SkillCraft.EntityFrameworkCore.Configurations;

internal class EducationConfiguration : AggregateConfiguration<EducationEntity>, IEntityTypeConfiguration<EducationEntity>
{
  public override void Configure(EntityTypeBuilder<EducationEntity> builder)
  {
    base.Configure(builder);

    builder.ToTable(SkillCraftDb.Educations.Table.Table ?? string.Empty, SkillCraftDb.Educations.Table.Schema);
    builder.HasKey(x => x.EducationId);

    builder.HasIndex(x => x.Id).IsUnique();
    builder.HasIndex(x => x.Name);

    builder.Property(x => x.Name).HasMaxLength(Slug.MaximumLength);
    builder.Property(x => x.Skill).HasMaxLength(byte.MaxValue).HasConversion(new EnumToStringConverter<Skill>());

    builder.HasOne(x => x.World).WithMany(x => x.Educations)
      .HasPrincipalKey(x => x.WorldId).HasForeignKey(x => x.WorldId)
      .OnDelete(DeleteBehavior.Restrict);
  }
}