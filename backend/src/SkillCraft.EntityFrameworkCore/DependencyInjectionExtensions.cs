﻿using Microsoft.Extensions.DependencyInjection;
using SkillCraft.Application.Actors;
using SkillCraft.Application.Aspects;
using SkillCraft.Application.Castes;
using SkillCraft.Application.Characters;
using SkillCraft.Application.Customizations;
using SkillCraft.Application.Educations;
using SkillCraft.Application.Languages;
using SkillCraft.Application.Lineages;
using SkillCraft.Application.Parties;
using SkillCraft.Application.Permissions;
using SkillCraft.Application.Personalities;
using SkillCraft.Application.Talents;
using SkillCraft.Application.Worlds;
using SkillCraft.Domain.Aspects;
using SkillCraft.Domain.Castes;
using SkillCraft.Domain.Characters;
using SkillCraft.Domain.Customizations;
using SkillCraft.Domain.Educations;
using SkillCraft.Domain.Languages;
using SkillCraft.Domain.Lineages;
using SkillCraft.Domain.Parties;
using SkillCraft.Domain.Personalities;
using SkillCraft.Domain.Storages;
using SkillCraft.Domain.Talents;
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
      .AddScoped<ICharacterQuerier, CharacterQuerier>()
      .AddScoped<ICustomizationQuerier, CustomizationQuerier>()
      .AddScoped<IEducationQuerier, EducationQuerier>()
      .AddScoped<ILanguageQuerier, LanguageQuerier>()
      .AddScoped<ILineageQuerier, LineageQuerier>()
      .AddScoped<IPartyQuerier, PartyQuerier>()
      .AddScoped<IPermissionQuerier, PermissionQuerier>()
      .AddScoped<IPersonalityQuerier, PersonalityQuerier>()
      .AddScoped<ITalentQuerier, TalentQuerier>()
      .AddScoped<IWorldQuerier, WorldQuerier>();
  }

  private static IServiceCollection AddRepositories(this IServiceCollection services)
  {
    return services
      .AddScoped<IAspectRepository, AspectRepository>()
      .AddScoped<ICasteRepository, CasteRepository>()
      .AddScoped<ICharacterRepository, CharacterRepository>()
      .AddScoped<ICustomizationRepository, CustomizationRepository>()
      .AddScoped<IEducationRepository, EducationRepository>()
      .AddScoped<ILanguageRepository, LanguageRepository>()
      .AddScoped<ILineageRepository, LineageRepository>()
      .AddScoped<IPartyRepository, PartyRepository>()
      .AddScoped<IPersonalityRepository, PersonalityRepository>()
      .AddScoped<IStorageRepository, StorageRepository>()
      .AddScoped<ITalentRepository, TalentRepository>()
      .AddScoped<IWorldRepository, WorldRepository>();
  }
}
