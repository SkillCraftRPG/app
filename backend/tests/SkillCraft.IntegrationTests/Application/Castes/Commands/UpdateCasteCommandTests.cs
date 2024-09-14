using Logitar;
using Microsoft.Extensions.DependencyInjection;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Castes;
using SkillCraft.Domain;
using SkillCraft.Domain.Castes;

namespace SkillCraft.Application.Castes.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class UpdateCasteCommandTests : IntegrationTests
{
  private readonly ICasteRepository _casteRepository;

  public UpdateCasteCommandTests() : base()
  {
    _casteRepository = ServiceProvider.GetRequiredService<ICasteRepository>();
  }

  [Fact(DisplayName = "It should update an existing caste.")]
  public async Task It_should_update_an_existing_caste()
  {
    Caste caste = new(World.Id, new Name("artisan"), World.OwnerId);
    Guid professionalId = Guid.NewGuid();
    caste.SetTrait(professionalId, new Trait(new Name("professional"), Description: null));
    Guid protegeId = Guid.NewGuid();
    caste.SetTrait(protegeId, new Trait(new Name("Protégé"), Description: null));
    await _casteRepository.SaveAsync(caste);

    UpdateCastePayload payload = new()
    {
      Name = "Artisan",
      Description = new Change<string>("    "),
      Skill = new Change<Skill?>(Skill.Craft),
      WealthRoll = new Change<string>("8d6"),
      Traits =
      [
        new UpdateTraitPayload("Sujet")
        {
          Description = "Sujet d’un seigneur quelconque, le personnage n’est victime d’aucune taxe imposée aux voyageurs étrangers. Il peut réduire ses dépenses essentielles de 10 % sur sa terre natale."
        },
        new UpdateTraitPayload("Professionnel")
        {
          Id = professionalId,
          Description = "Les apprentissages et réalisations du personnage lui ont permis de devenir membre d’une organisation de professionnels comme lui, telle une guilde d’artisans ou de marchands. S’il ne peut payer pour un toit ou de la nourriture, il peut facilement trouver du travail afin de couvrir ces dépenses essentielles."
        },
        new UpdateTraitPayload("Protégé")
        {
          Id = protegeId,
          Remove = true
        }
      ]
    };
    UpdateCasteCommand command = new(caste.Id.ToGuid(), payload);

    CasteModel? model = await Pipeline.ExecuteAsync(command, CancellationToken);
    Assert.NotNull(model);

    Assert.Equal(caste.Id.ToGuid(), model.Id);
    Assert.Equal(caste.Version + 1, model.Version);
    Assert.Equal(Actor, model.CreatedBy);
    Assert.Equal(Actor, model.UpdatedBy);
    Assert.Equal(caste.CreatedOn.AsUniversalTime(), model.CreatedOn);
    Assert.Equal(DateTime.UtcNow, model.UpdatedOn, TimeSpan.FromSeconds(1));

    Assert.Equal(payload.Name.Trim(), model.Name);
    Assert.Null(model.Description);
    Assert.Equal(payload.Skill.Value, model.Skill);
    Assert.Equal(payload.WealthRoll.Value, model.WealthRoll);

    Assert.Equal(2, model.Traits.Count);
    Assert.Contains(model.Traits, t => t.Id == professionalId && t.Name == "Professionnel" && t.Description == "Les apprentissages et réalisations du personnage lui ont permis de devenir membre d’une organisation de professionnels comme lui, telle une guilde d’artisans ou de marchands. S’il ne peut payer pour un toit ou de la nourriture, il peut facilement trouver du travail afin de couvrir ces dépenses essentielles.");
    Assert.Contains(model.Traits, t => t.Name == "Sujet" && t.Description == "Sujet d’un seigneur quelconque, le personnage n’est victime d’aucune taxe imposée aux voyageurs étrangers. Il peut réduire ses dépenses essentielles de 10 % sur sa terre natale.");
  }
}
