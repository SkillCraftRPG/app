import type { Attribute, Speed, Statistic } from "@/types/game";
import type { CharacterAttributes, CharacterModel, CharacterSpeeds, CharacterStatistics } from "@/types/characters";
import { calculateModifier } from "./gameUtils";

export function calculateAttributes(character: CharacterModel): CharacterAttributes {
  const scores = new Map<Attribute, number>([
    ["Agility", character.baseAttributes.agility],
    ["Coordination", character.baseAttributes.coordination],
    ["Intellect", character.baseAttributes.intellect],
    ["Presence", character.baseAttributes.presence],
    ["Sensitivity", character.baseAttributes.sensitivity],
    ["Spirit", character.baseAttributes.spirit],
    ["Vigor", character.baseAttributes.vigor],
  ]);

  scores.set("Agility", (scores.get("Agility") ?? 0) + character.lineage.attributes.agility);
  scores.set("Coordination", (scores.get("Coordination") ?? 0) + character.lineage.attributes.coordination);
  scores.set("Intellect", (scores.get("Intellect") ?? 0) + character.lineage.attributes.intellect);
  scores.set("Presence", (scores.get("Presence") ?? 0) + character.lineage.attributes.presence);
  scores.set("Sensitivity", (scores.get("Sensitivity") ?? 0) + character.lineage.attributes.sensitivity);
  scores.set("Spirit", (scores.get("Spirit") ?? 0) + character.lineage.attributes.spirit);
  scores.set("Vigor", (scores.get("Vigor") ?? 0) + character.lineage.attributes.vigor);
  if (character.lineage.species) {
    scores.set("Agility", (scores.get("Agility") ?? 0) + character.lineage.species.attributes.agility);
    scores.set("Coordination", (scores.get("Coordination") ?? 0) + character.lineage.species.attributes.coordination);
    scores.set("Intellect", (scores.get("Intellect") ?? 0) + character.lineage.species.attributes.intellect);
    scores.set("Presence", (scores.get("Presence") ?? 0) + character.lineage.species.attributes.presence);
    scores.set("Sensitivity", (scores.get("Sensitivity") ?? 0) + character.lineage.species.attributes.sensitivity);
    scores.set("Spirit", (scores.get("Spirit") ?? 0) + character.lineage.species.attributes.spirit);
    scores.set("Vigor", (scores.get("Vigor") ?? 0) + character.lineage.species.attributes.vigor);
  }
  character.baseAttributes.extra.forEach((attribute) => scores.set(attribute, (scores.get(attribute) ?? 0) + 1));

  if (character.nature.attribute) {
    scores.set(character.nature.attribute, (scores.get(character.nature.attribute) ?? 0) + 1);
  }

  scores.set(character.baseAttributes.best, (scores.get(character.baseAttributes.best) ?? 0) + 3);
  scores.set(character.baseAttributes.worst, (scores.get(character.baseAttributes.worst) ?? 0) + 1);
  character.baseAttributes.mandatory.forEach((attribute) => scores.set(attribute, (scores.get(attribute) ?? 0) + 2));
  character.baseAttributes.optional.forEach((attribute) => scores.set(attribute, (scores.get(attribute) ?? 0) + 1));

  character.levelUps.forEach(({ attribute }) => scores.set(attribute, (scores.get(attribute) ?? 0) + 1));

  const temporaryScores = new Map<Attribute, number>(scores.entries());
  character.bonuses.forEach((bonus) => {
    if (bonus.category === "Attribute") {
      const attribute: Attribute = bonus.target as Attribute;
      scores.set(attribute, (scores.get(attribute) ?? 0) + bonus.value);
      if (bonus.isTemporary) {
        temporaryScores.set(attribute, (scores.get(attribute) ?? 0) + bonus.value);
      }
    }
  });

  return {
    agility: {
      score: scores.get("Agility") ?? 0,
      modifier: calculateModifier(scores.get("Agility") ?? 0),
      temporaryScore: temporaryScores.get("Agility") ?? 0,
      temporaryModifier: calculateModifier(temporaryScores.get("Agility") ?? 0),
    },
    coordination: {
      score: scores.get("Coordination") ?? 0,
      modifier: calculateModifier(scores.get("Coordination") ?? 0),
      temporaryScore: temporaryScores.get("Coordination") ?? 0,
      temporaryModifier: calculateModifier(temporaryScores.get("Coordination") ?? 0),
    },
    intellect: {
      score: scores.get("Intellect") ?? 0,
      modifier: calculateModifier(scores.get("Intellect") ?? 0),
      temporaryScore: temporaryScores.get("Intellect") ?? 0,
      temporaryModifier: calculateModifier(temporaryScores.get("Intellect") ?? 0),
    },
    presence: {
      score: scores.get("Presence") ?? 0,
      modifier: calculateModifier(scores.get("Presence") ?? 0),
      temporaryScore: temporaryScores.get("Presence") ?? 0,
      temporaryModifier: calculateModifier(temporaryScores.get("Presence") ?? 0),
    },
    sensitivity: {
      score: scores.get("Sensitivity") ?? 0,
      modifier: calculateModifier(scores.get("Sensitivity") ?? 0),
      temporaryScore: temporaryScores.get("Sensitivity") ?? 0,
      temporaryModifier: calculateModifier(temporaryScores.get("Sensitivity") ?? 0),
    },
    spirit: {
      score: scores.get("Spirit") ?? 0,
      modifier: calculateModifier(scores.get("Spirit") ?? 0),
      temporaryScore: temporaryScores.get("Spirit") ?? 0,
      temporaryModifier: calculateModifier(temporaryScores.get("Spirit") ?? 0),
    },
    vigor: {
      score: scores.get("Vigor") ?? 0,
      modifier: calculateModifier(scores.get("Vigor") ?? 0),
      temporaryScore: temporaryScores.get("Vigor") ?? 0,
      temporaryModifier: calculateModifier(temporaryScores.get("Vigor") ?? 0),
    },
  };
} // TODO(fpion): unit tests

export function calculateSpeeds(character: CharacterModel): CharacterSpeeds {
  const speeds = new Map<Speed, number>([
    ["Walk", character.lineage.speeds.walk],
    ["Climb", character.lineage.speeds.climb],
    ["Swim", character.lineage.speeds.swim],
    ["Fly", character.lineage.speeds.fly],
    ["Hover", character.lineage.speeds.hover],
    ["Burrow", character.lineage.speeds.burrow],
  ]);
  if (character.lineage.species) {
    speeds.set("Walk", Math.max(character.lineage.species.speeds.walk, speeds.get("Walk") ?? 0));
    speeds.set("Climb", Math.max(character.lineage.species.speeds.climb, speeds.get("Climb") ?? 0));
    speeds.set("Swim", Math.max(character.lineage.species.speeds.swim, speeds.get("Swim") ?? 0));
    speeds.set("Fly", Math.max(character.lineage.species.speeds.fly, speeds.get("Fly") ?? 0));
    speeds.set("Hover", Math.max(character.lineage.species.speeds.hover, speeds.get("Hover") ?? 0));
    speeds.set("Burrow", Math.max(character.lineage.species.speeds.burrow, speeds.get("Burrow") ?? 0));
  }
  character.bonuses.forEach((bonus) => {
    if (bonus.category === "Speed") {
      const speed = bonus.target as Speed;
      speeds.set(speed, (speeds.get(speed) ?? 0) + bonus.value);
    }
  });
  return {
    walk: speeds.get("Walk") ?? 0,
    climb: speeds.get("Climb") ?? 0,
    swim: speeds.get("Swim") ?? 0,
    fly: speeds.get("Fly") ?? 0,
    hover: speeds.get("Hover") ?? 0,
    burrow: speeds.get("Burrow") ?? 0,
  };
} // TODO(fpion): unit tests

export function calculateStatistics(character: CharacterModel, attributes?: CharacterAttributes): CharacterStatistics {
  attributes ??= calculateAttributes(character);
  const bases = new Map<Statistic, number>([
    ["Constitution", 5 * (attributes.vigor.modifier + 5)],
    ["Initiative", attributes.sensitivity.modifier],
    ["Learning", Math.max(5, 2 * attributes.intellect.modifier + 5)],
    ["Power", attributes.spirit.modifier],
    ["Precision", attributes.coordination.modifier],
    ["Reputation", attributes.presence.modifier],
    ["Strength", attributes.agility.modifier],
  ]);
  const totals = new Map<Statistic, number>(bases.entries());
  character.levelUps.forEach((levelUp) => {
    totals.set("Constitution", (totals.get("Constitution") ?? 0) + levelUp.constitution);
    totals.set("Initiative", (totals.get("Initiative") ?? 0) + levelUp.initiative);
    totals.set("Learning", (totals.get("Learning") ?? 0) + levelUp.learning);
    totals.set("Power", (totals.get("Power") ?? 0) + levelUp.power);
    totals.set("Precision", (totals.get("Precision") ?? 0) + levelUp.precision);
    totals.set("Reputation", (totals.get("Reputation") ?? 0) + levelUp.reputation);
    totals.set("Strength", (totals.get("Strength") ?? 0) + levelUp.strength);
  });
  character.bonuses.forEach(({ category, target, value }) => {
    if (category === "Statistic") {
      const statistic = target as Statistic;
      totals.set(statistic, (totals.get(statistic) ?? 0) + value);
    }
  });
  return {
    constitution: {
      base: bases.get("Constitution") ?? 0,
      increment: attributes.vigor.modifier + 5,
      total: Math.floor(totals.get("Constitution") ?? 0),
    },
    initiative: {
      base: bases.get("Initiative") ?? 0,
      increment: attributes.sensitivity.score / 40,
      total: Math.floor(totals.get("Initiative") ?? 0),
    },
    learning: {
      base: bases.get("Learning") ?? 0,
      increment: Math.max(1, attributes.intellect.modifier + 2),
      total: Math.floor(totals.get("Learning") ?? 0),
    },
    power: {
      base: bases.get("Power") ?? 0,
      increment: attributes.spirit.score / 40,
      total: Math.floor(totals.get("Power") ?? 0),
    },
    precision: {
      base: bases.get("Precision") ?? 0,
      increment: attributes.coordination.score / 40,
      total: Math.floor(totals.get("Precision") ?? 0),
    },
    reputation: {
      base: bases.get("Reputation") ?? 0,
      increment: attributes.presence.score / 20,
      total: Math.floor(totals.get("Reputation") ?? 0),
    },
    strength: {
      base: bases.get("Strength") ?? 0,
      increment: attributes.agility.score / 40,
      total: Math.floor(totals.get("Strength") ?? 0),
    },
  };
} // TODO(fpion): unit tests

// TODO(fpion): should not all calculations be made by the backend?
