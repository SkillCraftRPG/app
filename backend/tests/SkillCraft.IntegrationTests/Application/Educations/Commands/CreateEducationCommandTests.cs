using SkillCraft.Contracts;
using SkillCraft.Contracts.Educations;

namespace SkillCraft.Application.Educations.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class CreateEducationCommandTests : IntegrationTests
{
  public CreateEducationCommandTests() : base()
  {
  }

  [Fact(DisplayName = "It should create a new education.", Skip = "Need to rethink integration tests.")]
  public async Task It_should_create_a_new_education()
  {
    CreateEducationPayload payload = new("Classique")
    {
      Description = "    ",
      Skill = Skill.Knowledge,
      WealthMultiplier = 12.0
    };
    CreateEducationCommand command = new(payload);

    var model = await Pipeline.ExecuteAsync(command, CancellationToken);

    Assert.NotEqual(Guid.Empty, model.Id);
    Assert.Equal(2, model.Version);
    Assert.Equal(Actor, model.CreatedBy);
    Assert.Equal(Actor, model.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, model.CreatedOn, TimeSpan.FromSeconds(1));
    Assert.True(model.CreatedOn < model.UpdatedOn);

    Assert.Equal(payload.Name.Trim(), model.Name);
    Assert.Null(model.Description);
    Assert.Equal(payload.Skill, model.Skill);
    Assert.Equal(payload.WealthMultiplier, model.WealthMultiplier);
  }
}
