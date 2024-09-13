using Logitar;
using Microsoft.Extensions.DependencyInjection;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Educations;
using SkillCraft.Domain;
using SkillCraft.Domain.Educations;

namespace SkillCraft.Application.Educations.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class UpdateEducationCommandTests : IntegrationTests
{
  private readonly IEducationRepository _educationRepository;

  public UpdateEducationCommandTests() : base()
  {
    _educationRepository = ServiceProvider.GetRequiredService<IEducationRepository>();
  }

  [Fact(DisplayName = "It should update an existing education.")]
  public async Task It_should_update_an_existing_education()
  {
    Education education = new(World.Id, new Name("classic"), World.OwnerId);
    await _educationRepository.SaveAsync(education);

    UpdateEducationPayload payload = new()
    {
      Name = "Classique",
      Description = new Change<string>("    "),
      Skill = new Change<Skill?>(Skill.Knowledge),
      WealthMultiplier = new Change<double?>(12.0)
    };
    UpdateEducationCommand command = new(education.Id.ToGuid(), payload);

    EducationModel? model = await Pipeline.ExecuteAsync(command, CancellationToken);
    Assert.NotNull(model);

    Assert.Equal(education.Id.ToGuid(), model.Id);
    Assert.Equal(education.Version + 1, model.Version);
    Assert.Equal(Actor, model.CreatedBy);
    Assert.Equal(Actor, model.UpdatedBy);
    Assert.Equal(education.CreatedOn.AsUniversalTime(), model.CreatedOn);
    Assert.Equal(DateTime.UtcNow, model.UpdatedOn, TimeSpan.FromSeconds(1));

    Assert.Equal(payload.Name.Trim(), model.Name);
    Assert.Null(model.Description);
    Assert.Equal(payload.Skill.Value, model.Skill);
    Assert.Equal(payload.WealthMultiplier.Value, model.WealthMultiplier);
  }
}
