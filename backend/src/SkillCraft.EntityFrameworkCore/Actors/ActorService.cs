using Logitar.Portal.Contracts.Users;
using Microsoft.EntityFrameworkCore;
using SkillCraft.Application.Actors;
using SkillCraft.EntityFrameworkCore.Entities;

namespace SkillCraft.EntityFrameworkCore.Actors;

internal class ActorService : IActorService
{
  private readonly SkillCraftContext _context;

  public ActorService(SkillCraftContext context)
  {
    _context = context;
  }

  public async Task SaveAsync(User user, CancellationToken cancellationToken)
  {
    ActorEntity? actor = await _context.Actors.SingleOrDefaultAsync(x => x.Id == user.Id, cancellationToken);
    if (actor == null)
    {
      actor = new(user);
      _context.Actors.Add(actor);
    }
    else
    {
      actor.Update(user);
    }

    await _context.SaveChangesAsync(cancellationToken);
  }
}
