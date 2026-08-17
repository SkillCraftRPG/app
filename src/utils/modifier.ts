import type { CharacterModifier } from "@/types/characters";

export function translateKind(modifier: CharacterModifier, translate: (key: string) => string): string {
  switch (modifier.kind) {
    case "Attribute":
      return translate("game.attribute.label");
    case "Skill":
      return translate("game.skill.label");
    case "Speed":
      return translate("game.speed.label");
    case "Statistic":
      return translate("game.statistic.label");
  }
}

export function translateTarget(modifier: CharacterModifier, translate: (key: string) => string): string {
  switch (modifier.kind) {
    case "Attribute":
      return translate(`game.attribute.options.${modifier.target}`);
    case "Skill":
      return translate(`game.skill.options.${modifier.target}`);
    case "Speed":
      return translate(`game.speed.kind.options.${modifier.target}`);
    case "Statistic":
      return translate(`game.statistic.options.${modifier.target}`);
  }
}
