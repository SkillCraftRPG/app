using Microsoft.EntityFrameworkCore;
using SkillCraft.EntityFrameworkCore.Entities;

namespace SkillCraft.EntityFrameworkCore;

public class SkillCraftContext : DbContext
{
  public SkillCraftContext(DbContextOptions<SkillCraftContext> options) : base(options)
  {
  }

  internal DbSet<UserEntity> Users { get; private set; }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
  }
}
