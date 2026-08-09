import { defineStore } from "pinia";
import { ref } from "vue";

import type { Caste } from "@/types/castes";
import type {
  Alignment,
  CharacterAppearanceDetail,
  CharacterCreation,
  CharacterPersonality,
  CharacterTalent,
  DominantHand,
  SkillRankPayload,
  StartingAttributes,
} from "@/types/characters";
import type { Customization } from "@/types/customizations";
import type { Education } from "@/types/educations";
import type { Language } from "@/types/languages";
import type { Lineage } from "@/types/lineages";
import { CharacterCreationStep } from "@/types/characters";

function defaultCreation(): CharacterCreation {
  return {
    languages: [],
    name: "",
    customizations: [],
    talents: [],
    attributes: { dexterity: 0, health: 0, intellect: 0, senses: 0, vigor: 0 },
    skills: [],
    appearance: { height: 0, weightCategory: "", bodyMassIndex: 0, age: 0, skin: "", eyes: "", hair: "" },
    personality: {},
    background: "",
  };
}

export const useCharacterStore = defineStore(
  "character",
  () => {
    const creation = ref<CharacterCreation>(defaultCreation());
    const step = ref<CharacterCreationStep>(CharacterCreationStep.Ascendancy);

    function abandon(): void {
      creation.value = defaultCreation();
      step.value = CharacterCreationStep.Ascendancy;
    }

    function goBack(): void {
      if (step.value !== CharacterCreationStep.Ascendancy) {
        step.value--;
      }
    }

    function saveAppearance(appearance: CharacterAppearanceDetail): void {
      creation.value.appearance = appearance;
      step.value++;
    }

    function saveAscendancy(species: Lineage, languages: Language[], ethnicity?: Lineage): void {
      creation.value.species = species;
      creation.value.ethnicity = ethnicity;
      creation.value.languages = [...languages];
      step.value++;
    }

    function saveAttributes(attributes: StartingAttributes): void {
      creation.value.attributes = { ...attributes };
      step.value++;
    }

    function saveBackground(background: string): void {
      creation.value.background = background;
      step.value++;
    }

    function saveContext(caste: Caste, education: Education): void {
      creation.value.caste = caste;
      creation.value.education = education;
      step.value++;
    }

    function saveCustomization(name: string, customizations: Customization[], dominantHand?: DominantHand | null): void {
      creation.value.name = name;
      creation.value.dominantHand = dominantHand;
      creation.value.customizations = [...customizations];
      step.value++;
    }

    function savePersonality(alignment: Alignment | null, personality: CharacterPersonality): void {
      creation.value.alignment = alignment;
      creation.value.personality = personality;
      step.value++;
    }

    function saveSkills(skills: SkillRankPayload[]): void {
      creation.value.skills = [...skills];
      step.value++;
    }

    function saveTalents(talents: CharacterTalent[]): void {
      creation.value.talents = [...talents];
      step.value++;
    }

    return {
      creation,
      step,
      abandon,
      goBack,
      saveAppearance,
      saveAscendancy,
      saveAttributes,
      saveBackground,
      saveContext,
      saveCustomization,
      savePersonality,
      saveSkills,
      saveTalents,
    };
  },
  { persist: true },
);
