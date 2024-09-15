﻿using Microsoft.Extensions.DependencyInjection;
using SkillCraft.Application.Actors;
using SkillCraft.Application.Aspects;
using SkillCraft.Application.Castes;
using SkillCraft.Application.Customizations;
using SkillCraft.Application.Educations;
using SkillCraft.Application.Languages;
using SkillCraft.Application.Worlds;
using SkillCraft.Domain.Aspects;
using SkillCraft.Domain.Castes;
using SkillCraft.Domain.Customizations;
using SkillCraft.Domain.Educations;
using SkillCraft.Domain.Languages;
using SkillCraft.Domain.Storages;
using SkillCraft.Domain.Worlds;
using SkillCraft.EntityFrameworkCore.Actors;
using SkillCraft.EntityFrameworkCore.Queriers;
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
    return services
      .AddScoped<IAspectQuerier, AspectQuerier>()
      .AddScoped<ICasteQuerier, CasteQuerier>()
      .AddScoped<ICustomizationQuerier, CustomizationQuerier>()
      .AddScoped<IEducationQuerier, EducationQuerier>()
      .AddScoped<ILanguageQuerier, LanguageQuerier>()
      .AddScoped<IWorldQuerier, WorldQuerier>();
  }

  private static IServiceCollection AddRepositories(this IServiceCollection services)
  {
    return services
      .AddScoped<IAspectRepository, AspectRepository>()
      .AddScoped<ICasteRepository, CasteRepository>()
      .AddScoped<ICustomizationRepository, CustomizationRepository>()
      .AddScoped<IEducationRepository, EducationRepository>()
      .AddScoped<ILanguageRepository, LanguageRepository>()
      .AddScoped<IStorageRepository, StorageRepository>()
      .AddScoped<IWorldRepository, WorldRepository>();
  }
}
