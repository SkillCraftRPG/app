using Logitar.EventSourcing;
using Logitar.Portal.Contracts.Actors;
using Microsoft.Extensions.Caching.Memory;
using SkillCraft.Application.Caching;

namespace SkillCraft.Infrastructure.Caching;

internal class CacheService : ICacheService
{
  private readonly IMemoryCache _memoryCache;

  public CacheService(IMemoryCache memoryCache)
  {
    _memoryCache = memoryCache;
  }

  public Actor? GetActor(ActorId id)
  {
    string key = GetActorKey(id);
    return GetItem<Actor>(key);
  }
  public void SetActor(Actor actor)
  {
    string key = GetActorKey(new ActorId(actor.Id));
    SetItem(key, actor);
  }
  private static string GetActorKey(ActorId id) => $"Actor.Id:{id}";

  private T? GetItem<T>(object key) => _memoryCache.TryGetValue(key, out object? value) ? (T?)value : default;
  private void SetItem<T>(object key, T value) => _memoryCache.Set(key, value);
}
