﻿using Logitar.EventSourcing;
using Logitar.Portal.Contracts.Actors;

namespace SkillCraft.Application.Caching;

public interface ICacheService
{
  Actor? GetActor(ActorId id);
  void SetActor(Actor actor);
}
