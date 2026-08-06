import { defineStore } from "pinia";
import { ref } from "vue";

import type { Caste } from "@/types/castes";
import type { CharacterCreation, CharacterTalent, DominantHand } from "@/types/characters";
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

    function saveAscendancy(species: Lineage, languages: Language[], ethnicity?: Lineage): void {
      creation.value.species = species;
      creation.value.ethnicity = ethnicity;
      creation.value.languages = [...languages];
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

    function saveTalents(talents: CharacterTalent[]): void {
      creation.value.talents = [...talents];
      step.value++;
    }

    return { creation, step, abandon, goBack, saveAscendancy, saveContext, saveCustomization, saveTalents };
  },
  { persist: true },
);
