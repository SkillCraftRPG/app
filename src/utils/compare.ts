import type { Caste } from "@/types/castes";
import type { Customization } from "@/types/customizations";
import type { Education } from "@/types/educations";
import type { Feature } from "@/types/features";
import type { Language } from "@/types/languages";
import type { Lineage, LineageAge, LineageNames, LineageSize, LineageSpeeds, LineageWeight, NameCategory } from "@/types/lineages";
import type { Script } from "@/types/scripts";
import type { Talent } from "@/types/talents";

export function compareCastes(left: Caste, right: Caste): boolean {
  return (
    left.name === right.name &&
    (left.summary ?? "") === (right.summary ?? "") &&
    (left.content ?? "") === (right.content ?? "") &&
    (left.skill ?? "") === (right.skill ?? "") &&
    (left.wealthRoll ?? "") === (right.wealthRoll ?? "") &&
    (left.feature?.name ?? "") === (right.feature?.name ?? "") &&
    (left.feature?.content ?? "") === (right.feature?.content ?? "")
  );
}

export function compareCustomizations(left: Customization, right: Customization): boolean {
  return left.name === right.name && (left.summary ?? "") === (right.summary ?? "") && (left.content ?? "") === (right.content ?? "");
}

export function compareEducations(left: Education, right: Education): boolean {
  return (
    left.name === right.name &&
    (left.summary ?? "") === (right.summary ?? "") &&
    (left.content ?? "") === (right.content ?? "") &&
    (left.skill ?? "") === (right.skill ?? "") &&
    (left.wealthMultiplier ?? 0) === (right.wealthMultiplier ?? 0) &&
    (left.feature?.name ?? "") === (right.feature?.name ?? "") &&
    (left.feature?.content ?? "") === (right.feature?.content ?? "")
  );
}

export function compareLanguages(left: Language, right: Language): boolean {
  return (
    left.name === right.name &&
    (left.summary ?? "") === (right.summary ?? "") &&
    (left.content ?? "") === (right.content ?? "") &&
    (left.script?.id ?? "") === (right.script?.id ?? "") &&
    (left.typicalSpeakers ?? "") === (right.typicalSpeakers ?? "")
  );
}

export function compareLineages(left: Lineage, right: Lineage): boolean {
  return (
    left.name === right.name &&
    (left.summary ?? "") === (right.summary ?? "") &&
    (left.content ?? "") === (right.content ?? "") &&
    (left.parent?.id ?? "") === (right.parent?.id ?? "") &&
    compareFeatures(left.features, right.features) &&
    left.languages.extra === right.languages.extra &&
    (left.languages.content ?? "") === (right.languages.content ?? "") &&
    equalIds(
      left.languages.granted.map(({ id }) => id),
      right.languages.granted.map(({ id }) => id),
    ) &&
    compareNames(left.names, right.names) &&
    compareSpeeds(left.speeds, right.speeds) &&
    compareSize(left.size, right.size) &&
    compareWeight(left.weight, right.weight) &&
    compareAge(left.age, right.age)
  );
}

export function compareScripts(left: Script, right: Script): boolean {
  return left.name === right.name && (left.summary ?? "") === (right.summary ?? "") && (left.content ?? "") === (right.content ?? "");
}

export function compareTalents(left: Talent, right: Talent): boolean {
  return (
    left.tier === right.tier &&
    left.name === right.name &&
    (left.summary ?? "") === (right.summary ?? "") &&
    (left.content ?? "") === (right.content ?? "") &&
    left.allowMultiplePurchases === right.allowMultiplePurchases &&
    (left.skill ?? "") === (right.skill ?? "") &&
    (left.requiredTalent?.id ?? "") === (right.requiredTalent?.id ?? "")
  );
}

function compareAge(left: LineageAge, right: LineageAge): boolean {
  return (
    (left.teenager ?? 0) === (right.teenager ?? 0) &&
    (left.adult ?? 0) === (right.adult ?? 0) &&
    (left.mature ?? 0) === (right.mature ?? 0) &&
    (left.venerable ?? 0) === (right.venerable ?? 0)
  );
}

function compareFeatures(left: Feature[], right: Feature[]): boolean {
  if (left.length !== right.length) {
    return false;
  }
  const sortedLeft = [...left].sort((a, b) => a.name.localeCompare(b.name));
  const sortedRight = [...right].sort((a, b) => a.name.localeCompare(b.name));
  return sortedLeft.every((feature, index) => {
    const other = sortedRight[index];
    if (!other) {
      return false;
    }
    return feature.name === other.name && (feature.content ?? "") === (other.content ?? "");
  });
}

function compareNameCategories(left: NameCategory[], right: NameCategory[]): boolean {
  if (left.length !== right.length) {
    return false;
  }
  const sortedLeft = [...left].sort((a, b) => a.category.localeCompare(b.category));
  const sortedRight = [...right].sort((a, b) => a.category.localeCompare(b.category));
  return sortedLeft.every((category, index) => {
    const other = sortedRight[index];
    if (!other) {
      return false;
    }
    return category.category === other.category && equalStrings(category.values, other.values);
  });
}

function compareNames(left: LineageNames, right: LineageNames): boolean {
  return (
    equalStrings(left.family, right.family) &&
    equalStrings(left.female, right.female) &&
    equalStrings(left.male, right.male) &&
    equalStrings(left.unisex, right.unisex) &&
    compareNameCategories(left.custom, right.custom) &&
    (left.content ?? "") === (right.content ?? "")
  );
}

function compareSize(left: LineageSize, right: LineageSize): boolean {
  return left.category === right.category && (left.height ?? "") === (right.height ?? "");
}

function compareSpeeds(left: LineageSpeeds, right: LineageSpeeds): boolean {
  return (
    (left.walk ?? 0) === (right.walk ?? 0) &&
    (left.climb ?? 0) === (right.climb ?? 0) &&
    (left.swim ?? 0) === (right.swim ?? 0) &&
    (left.fly ?? 0) === (right.fly ?? 0) &&
    left.hover === right.hover &&
    (left.burrow ?? 0) === (right.burrow ?? 0)
  );
}

function compareWeight(left: LineageWeight, right: LineageWeight): boolean {
  return (
    (left.malnutrition ?? "") === (right.malnutrition ?? "") &&
    (left.skinny ?? "") === (right.skinny ?? "") &&
    (left.normal ?? "") === (right.normal ?? "") &&
    (left.overweight ?? "") === (right.overweight ?? "") &&
    (left.obese ?? "") === (right.obese ?? "")
  );
}

function equalIds(left: string[], right: string[]): boolean {
  if (left.length !== right.length) {
    return false;
  }
  const sortedLeft = [...left].sort();
  const sortedRight = [...right].sort();
  return sortedLeft.every((id, index) => id === sortedRight[index]);
}

function equalStrings(left: string[], right: string[]): boolean {
  if (left.length !== right.length) {
    return false;
  }
  const sortedLeft = [...left].sort();
  const sortedRight = [...right].sort();
  return sortedLeft.every((value, index) => value === sortedRight[index]);
}
