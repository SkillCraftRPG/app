using Logitar;
using Microsoft.Extensions.DependencyInjection;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Castes;
using SkillCraft.Domain;
using SkillCraft.Domain.Castes;

namespace SkillCraft.Application.Castes.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class ReplaceCasteCommandTests : IntegrationTests
{
  private readonly ICasteRepository _casteRepository;

  public ReplaceCasteCommandTests() : base()
  {
    _casteRepository = ServiceProvider.GetRequiredService<ICasteRepository>();
  }

  [Fact(DisplayName = "It should replace an existing caste.")]
  public async Task It_should_replace_an_existingcaste()
  {
    Caste caste = new(World.Id, new Name("artisan"), World.OwnerId);
    Guid professionalId = Guid.NewGuid();
    caste.SetTrait(professionalId, new Trait(new Name("professional"), Description: null));
    caste.AddTrait(new Trait(new Name("protected"), Description: null));
    caste.Update(World.OwnerId);
    long version = caste.Version;
    await _casteRepository.SaveAsync(caste);

    Description description = new("L’artisan est un expert d’un procédé de transformation des matières brutes. Il peut être un boulanger, un forgeron, un orfèvre, un tisserand ou pratiquer tout genre de profession œuvrant dans la transformation des matières brutes.");
    caste.Description = description;
    caste.AddTrait(new Trait(new Name("Sujet"), new Description("Sujet d’un seigneur quelconque, le personnage n’est victime d’aucune taxe imposée aux voyageurs étrangers. Il peut réduire ses dépenses essentielles de 10 % sur sa terre natale.")));
    caste.Update(World.OwnerId);
    await _casteRepository.SaveAsync(caste);

    ReplaceCastePayload payload = new(" Artisan ")
    {
      Description = "    ",
      Skill = Skill.Craft,
      WealthRoll = "8d6",
      Traits =
      [
        new TraitPayload("Professionnel")
        {
          Id = professionalId,
          Description = "Les apprentissages et réalisations du personnage lui ont permis de devenir membre d’une organisation de professionnels comme lui, telle une guilde d’artisans ou de marchands. S’il ne peut payer pour un toit ou de la nourriture, il peut facilement trouver du travail afin de couvrir ces dépenses essentielles."
        }
      ]
    };
    ReplaceCasteCommand command = new(caste.Id.ToGuid(), payload, version);

    CasteModel? model = await Pipeline.ExecuteAsync(command, CancellationToken);
    Assert.NotNull(model);

    Assert.Equal(caste.Id.ToGuid(), model.Id);
    Assert.Equal(caste.Version + 1, model.Version);
    Assert.Equal(Actor, model.CreatedBy);
    Assert.Equal(Actor, model.UpdatedBy);
    Assert.Equal(caste.CreatedOn.AsUniversalTime(), model.CreatedOn);
    Assert.Equal(DateTime.UtcNow, model.UpdatedOn, TimeSpan.FromSeconds(1));

    Assert.Equal(payload.Name.Trim(), model.Name);
    Assert.Equal(description.Value, model.Description);
    Assert.Equal(payload.Skill, model.Skill);
    Assert.Equal(payload.WealthRoll, model.WealthRoll);

    Assert.Equal(2, model.Traits.Count);
    Assert.Contains(model.Traits, t => t.Id == professionalId && t.Name == "Professionnel" && t.Description == "Les apprentissages et réalisations du personnage lui ont permis de devenir membre d’une organisation de professionnels comme lui, telle une guilde d’artisans ou de marchands. S’il ne peut payer pour un toit ou de la nourriture, il peut facilement trouver du travail afin de couvrir ces dépenses essentielles.");
    Assert.Contains(model.Traits, t => t.Name == "Sujet" && t.Description == "Sujet d’un seigneur quelconque, le personnage n’est victime d’aucune taxe imposée aux voyageurs étrangers. Il peut réduire ses dépenses essentielles de 10 % sur sa terre natale.");
  }
}
