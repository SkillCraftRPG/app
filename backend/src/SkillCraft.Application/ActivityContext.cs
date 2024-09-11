using Logitar.Portal.Contracts.ApiKeys;
using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Contracts.Users;
using SkillCraft.Contracts.Worlds;

namespace SkillCraft.Application;

public record ActivityContext(ApiKey? ApiKey, Session? Session, User? User, WorldModel? World);
