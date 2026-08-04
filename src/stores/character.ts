import { defineStore } from "pinia";
import { ref } from "vue";

import type { Customization } from "@/types/customizations";
import type { Language } from "@/types/languages";
import type { Lineage } from "@/types/lineages";
import { CharacterCreationStep, type CreateCharacterPayload, type DominantHand } from "@/types/characters";

export const useCharacterStore = defineStore(
  "character",
  () => {
    const payload = ref<CreateCharacterPayload>({
      lineageId: "",
      languageIds: [],
      name: "",
      customizationIds: [],
    });
    const step = ref<CharacterCreationStep>(CharacterCreationStep.Ascendancy);

    function abandon(): void {
      payload.value.lineageId = "";
      payload.value.languageIds = [];
      payload.value.name = "";
      payload.value.dominantHand = null;
      payload.value.customizationIds = [];
      step.value = CharacterCreationStep.Ascendancy;
    }

    function goBack(): void {
      if (step.value !== CharacterCreationStep.Ascendancy) {
        step.value--;
      }
    }

    function saveAscendancy(lineage: Lineage, languages: Language[]): void {
      payload.value.lineageId = lineage.id;
      payload.value.languageIds = languages.map((language) => language.id);
      step.value++;
    }

    function saveCustomization(name: string, dominantHand: DominantHand | null, customizations: Customization[]): void {
      payload.value.name = name;
      payload.value.dominantHand = dominantHand;
      payload.value.customizationIds = customizations.map((customization) => customization.id);
      step.value++;
    }

    return { payload, step, abandon, goBack, saveAscendancy, saveCustomization };
  },
  { persist: true },
);
