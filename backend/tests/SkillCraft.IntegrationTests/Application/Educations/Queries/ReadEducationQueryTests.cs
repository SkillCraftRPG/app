using Microsoft.Extensions.DependencyInjection;
using SkillCraft.Contracts.Educations;
using SkillCraft.Domain;
using SkillCraft.Domain.Educations;

namespace SkillCraft.Application.Educations.Queries;

[Trait(Traits.Category, Categories.Integration)]
public class ReadEducationQueryTests : IntegrationTests
{
  private readonly IEducationRepository _educationRepository;

  public ReadEducationQueryTests() : base()
  {
    _educationRepository = ServiceProvider.GetRequiredService<IEducationRepository>();
  }

  [Fact(DisplayName = "It should return the correct education.", Skip = "Need to rethink integration tests.")]
  public async Task It_should_return_the_correct_education()
  {
    Education education = new(World.Id, new Name("Classique"), World.OwnerId);
    await _educationRepository.SaveAsync(education);

    ReadEducationQuery query = new(education.Id.ToGuid());

    EducationModel? model = await Pipeline.ExecuteAsync(query);
    Assert.NotNull(model);
    Assert.Equal(education.Id.ToGuid(), model.Id);
  }
}
