using Logitar;
using Microsoft.Extensions.DependencyInjection;
using SkillCraft.Contracts.Educations;
using SkillCraft.Domain;
using SkillCraft.Domain.Educations;

namespace SkillCraft.Application.Educations.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class ReplaceEducationCommandTests : IntegrationTests
{
  private readonly IEducationRepository _educationRepository;

  public ReplaceEducationCommandTests() : base()
  {
    _educationRepository = ServiceProvider.GetRequiredService<IEducationRepository>();
  }

  [Fact(DisplayName = "It should replace an existing education.")]
  public async Task It_should_replace_an_existingeducation()
  {
    Education education = new(World.Id, new Name("classic"), World.OwnerId);
    long version = education.Version;
    await _educationRepository.SaveAsync(education);

    Description description = new("Peu peuvent se vanter d’avoir reçu une éducation traditionnelle comme celle du personnage. Il a suivi un parcours scolaire conforme et sans dérogation ayant mené à une instruction de haute qualité. Malgré son manque d’expériences personnelles, son grand savoir lui permet de se débrouiller même dans les situations les plus difficiles.");
    education.Description = description;
    education.Update(World.OwnerId);
    await _educationRepository.SaveAsync(education);

    ReplaceEducationPayload payload = new(" Classique ")
    {
      Description = "    ",
      Skill = Contracts.Skill.Knowledge,
      WealthMultiplier = 12.0
    };
    ReplaceEducationCommand command = new(education.Id.ToGuid(), payload, version);

    EducationModel? model = await Pipeline.ExecuteAsync(command, CancellationToken);
    Assert.NotNull(model);

    Assert.Equal(education.Id.ToGuid(), model.Id);
    Assert.Equal(education.Version + 1, model.Version);
    Assert.Equal(Actor, model.CreatedBy);
    Assert.Equal(Actor, model.UpdatedBy);
    Assert.Equal(education.CreatedOn.AsUniversalTime(), model.CreatedOn);
    Assert.Equal(DateTime.UtcNow, model.UpdatedOn, TimeSpan.FromSeconds(1));

    Assert.Equal(payload.Name.Trim(), model.Name);
    Assert.Equal(description.Value, model.Description);
    Assert.Equal(payload.Skill, model.Skill);
    Assert.Equal(payload.WealthMultiplier, model.WealthMultiplier);
  }
}
