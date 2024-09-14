using Microsoft.Extensions.DependencyInjection;
using SkillCraft.Contracts.Castes;
using SkillCraft.Domain;
using SkillCraft.Domain.Castes;

namespace SkillCraft.Application.Castes.Queries;

[Trait(Traits.Category, Categories.Integration)]
public class ReadCasteQueryTests : IntegrationTests
{
  private readonly ICasteRepository _casteRepository;

  public ReadCasteQueryTests() : base()
  {
    _casteRepository = ServiceProvider.GetRequiredService<ICasteRepository>();
  }

  [Fact(DisplayName = "It should return the correct caste.")]
  public async Task It_should_return_the_correct_caste()
  {
    Caste caste = new(World.Id, new Name("Artisan"), World.OwnerId);
    await _casteRepository.SaveAsync(caste);

    ReadCasteQuery query = new(caste.Id.ToGuid());

    CasteModel? model = await Pipeline.ExecuteAsync(query);
    Assert.NotNull(model);
    Assert.Equal(caste.Id.ToGuid(), model.Id);
  }
}
