﻿using Microsoft.EntityFrameworkCore;
using SkillCraft.EntityFrameworkCore.Entities;

namespace SkillCraft.EntityFrameworkCore;

public class SkillCraftContext : DbContext
{
  public SkillCraftContext(DbContextOptions<SkillCraftContext> options) : base(options)
  {
  }

  internal DbSet<AspectEntity> Aspects { get; private set; }
  internal DbSet<CasteEntity> Castes { get; private set; }
  internal DbSet<CustomizationEntity> Customizations { get; private set; }
  internal DbSet<EducationEntity> Educations { get; private set; }
  internal DbSet<LanguageEntity> Languages { get; private set; }
  internal DbSet<LineageEntity> Lineages { get; private set; }
  internal DbSet<LineageLanguageEntity> LineageLanguages { get; private set; }
  internal DbSet<PartyEntity> Parties { get; private set; }
  internal DbSet<PersonalityEntity> Personalities { get; private set; }
  internal DbSet<StorageDetailEntity> StorageDetails { get; private set; }
  internal DbSet<StorageSummaryEntity> StorageSummaries { get; private set; }
  internal DbSet<TalentEntity> Talents { get; private set; }
  internal DbSet<UserEntity> Users { get; private set; }
  internal DbSet<WorldEntity> Worlds { get; private set; }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
  }
}
