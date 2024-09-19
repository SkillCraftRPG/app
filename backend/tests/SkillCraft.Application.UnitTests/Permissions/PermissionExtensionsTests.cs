using Bogus;
using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Contracts.Users;
using SkillCraft.Contracts.Worlds;
using SkillCraft.Domain;
using SkillCraft.Domain.Worlds;

namespace SkillCraft.Application.Permissions;

[Trait(Traits.Category, Categories.Unit)]
public class PermissionExtensionsTests
{
  private readonly Faker _faker = new();

  private readonly User _otherUser;
  private readonly UserMock _user;
  private readonly WorldModel _world;

  public PermissionExtensionsTests()
  {
    _otherUser = new(_faker.Internet.UserName())
    {
      Id = Guid.NewGuid(),
      Email = new Email(_faker.Internet.Email()),
      FirstName = _faker.Name.FirstName(),
      LastName = _faker.Name.LastName(),
      FullName = _faker.Name.FullName(),
      Picture = _faker.Internet.Avatar()
    };
    _user = new(_faker);

    _world = new(new Actor(_user), "ungar")
    {
      Id = Guid.NewGuid()
    };
  }

  [Fact(DisplayName = "IsOwner: it should return false when the user does not own the world.")]
  public void IsOwner_it_should_return_false_when_the_user_does_not_own_the_world()
  {
    Assert.False(_otherUser.IsOwner(_world));
  }

  [Fact(DisplayName = "IsOwner: it should return true when the user owns the world.")]
  public void IsOwner_it_should_return_true_when_the_user_owns_the_world()
  {
    Assert.True(_user.IsOwner(_world));
  }

  [Fact(DisplayName = "ResidesIn: it should return false when the entity does not reside in the world.")]
  public void ResidesIn_it_should_return_false_when_the_entity_does_not_reside_in_the_world()
  {
    EntityMetadata entity = new(WorldId.NewId(), new EntityKey(EntityType.Lineage, Guid.NewGuid()), size: 1);
    Assert.False(entity.ResidesIn(_world));
  }

  [Fact(DisplayName = "IsOwner: it should return true when the entity resides in the world.")]
  public void ResidesIn_it_should_return_true_when_the_entity_resides_in_the_world()
  {
    EntityMetadata entity = new(new WorldId(_world.Id), new EntityKey(EntityType.Customization, Guid.NewGuid()), size: 1);
    Assert.True(entity.ResidesIn(_world));
  }
}
