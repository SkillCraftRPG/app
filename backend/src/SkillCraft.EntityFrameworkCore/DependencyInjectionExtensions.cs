﻿using Microsoft.Extensions.DependencyInjection;
using SkillCraft.Application.Actors;
using SkillCraft.Application.Worlds;
using SkillCraft.Domain.Storage;
using SkillCraft.Domain.Worlds;
using SkillCraft.EntityFrameworkCore.Actors;
using SkillCraft.EntityFrameworkCore.Queries;
using SkillCraft.EntityFrameworkCore.Repositories;
using SkillCraft.Infrastructure;

namespace SkillCraft.EntityFrameworkCore;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddSkillCraftWithEntityFrameworkCore(this IServiceCollection services)
  {
    return services
      .AddMediatR(config => config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()))
      .AddSkillCraftInfrastructure()
      .AddQueriers()
      .AddRepositories()
      .AddScoped<IActorService, ActorService>();
  }

  private static IServiceCollection AddQueriers(this IServiceCollection services)
  {
    return services.AddTransient<IWorldQuerier, WorldQuerier>();
  }

  private static IServiceCollection AddRepositories(this IServiceCollection services)
  {
    return services
      .AddTransient<IStorageRepository, StorageRepository>()
      .AddTransient<IWorldRepository, WorldRepository>();
  }
}
