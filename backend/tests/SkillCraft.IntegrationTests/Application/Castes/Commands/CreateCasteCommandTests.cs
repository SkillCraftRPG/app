using SkillCraft.Contracts;
using SkillCraft.Contracts.Castes;

namespace SkillCraft.Application.Castes.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class CreateCasteCommandTests : IntegrationTests
{
  public CreateCasteCommandTests() : base()
  {
  }

  [Fact(DisplayName = "It should create a new caste.")]
  public async Task It_should_create_a_new_caste()
  {
    CreateCastePayload payload = new("Artisan")
    {
      Description = "    ",
      Skill = Skill.Craft,
      WealthRoll = "8d6",
      Traits =
      [
        new TraitPayload
        {
          Name = "Professionnel",
          Description = "Les apprentissages et réalisations du personnage lui ont permis de devenir membre d’une organisation de professionnels comme lui, telle une guilde d’artisans ou de marchands. S’il ne peut payer pour un toit ou de la nourriture, il peut facilement trouver du travail afin de couvrir ces dépenses essentielles."
        },
        new TraitPayload
        {
          Name = "Sujet",
          Description = "Sujet d’un seigneur quelconque, le personnage n’est victime d’aucune taxe imposée aux voyageurs étrangers. Il peut réduire ses dépenses essentielles de 10 % sur sa terre natale."
        }
      ]
    };
    CreateCasteCommand command = new(payload);

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
    Assert.Equal(payload.WealthRoll, model.WealthRoll);

    Assert.Equal(payload.Traits.Count, model.Traits.Count);
    foreach (TraitPayload trait in payload.Traits)
    {
      Assert.Contains(model.Traits, t => t.Name == trait.Name.Trim() && t.Description == trait.Description?.Trim());
    }
  }
}
