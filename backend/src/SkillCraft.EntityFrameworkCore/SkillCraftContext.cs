using Microsoft.EntityFrameworkCore;
using SkillCraft.EntityFrameworkCore.Entities;

namespace SkillCraft.EntityFrameworkCore;

public class SkillCraftContext : DbContext
{
  public SkillCraftContext(DbContextOptions<SkillCraftContext> options) : base(options)
  {
  }

  internal DbSet<EducationEntity> Educations { get; private set; }
  internal DbSet<StorageDetailEntity> StorageDetails { get; private set; }
  internal DbSet<StorageSummaryEntity> StorageSummaries { get; private set; }
  internal DbSet<UserEntity> Users { get; private set; }
  internal DbSet<WorldEntity> Worlds { get; private set; }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
  }
}
