using FluentValidation;
using SkillCraft.Domain.Characters.Validators;
using Attribute = SkillCraft.Contracts.Attribute;

namespace SkillCraft.Domain.Characters;

public record BaseAttributes // TODO(fpion): serialization tests
{
  public const int MinimumScore = 6;
  public const int MaximumScore = 11;
  public const int ScoreSum = 7 /* attributes */ * 8 /* points */ + 1 /* point */;

  public int Agility { get; }
  public int Coordination { get; }
  public int Intellect { get; }
  public int Presence { get; }
  public int Sensitivity { get; }
  public int Spirit { get; }
  public int Vigor { get; }

  public Attribute Best { get; }
  public Attribute Worst { get; }
  public IReadOnlyCollection<Attribute> Mandatory { get; }
  public IReadOnlyCollection<Attribute> Optional { get; }

  public IReadOnlyCollection<Attribute> Extra { get; }

  public BaseAttributes(
    int agility,
    int coordination,
    int intellect,
    int presence,
    int sensitivity,
    int spirit,
    int vigor,
    Attribute best,
    Attribute worst,
    IReadOnlyCollection<Attribute> mandatory,
    IReadOnlyCollection<Attribute> optional,
    IReadOnlyCollection<Attribute> extra)
  {
    Agility = agility;
    Coordination = coordination;
    Intellect = intellect;
    Presence = presence;
    Sensitivity = sensitivity;
    Spirit = spirit;
    Vigor = vigor;

    Best = best;
    Worst = worst;
    Mandatory = mandatory;
    Optional = optional;

    Extra = extra.Distinct().ToArray().AsReadOnly();

    new BaseAttributesValidator().ValidateAndThrow(this);
  }
}
