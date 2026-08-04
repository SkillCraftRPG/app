import { defineStore } from "pinia";
import { ref } from "vue";

import type { Language } from "@/types/languages";
import type { Lineage } from "@/types/lineages";
import { CharacterCreationStep, type CreateCharacterPayload } from "@/types/characters";

export const useCharacterStore = defineStore(
  "character",
  () => {
    const payload = ref<CreateCharacterPayload>({
      lineageId: "",
      languageIds: [],
    });
    const step = ref<CharacterCreationStep>(CharacterCreationStep.Ascendancy);

    function abandon(): void {
      payload.value.lineageId = "";
      payload.value.languageIds = [];
      step.value = CharacterCreationStep.Ascendancy;
    }

    function saveAscendancy(lineage: Lineage, languages: Language[]): void {
      payload.value.lineageId = lineage.id;
      payload.value.languageIds = languages.map((language) => language.id);
      step.value++;
    }

    return { payload, step, abandon, saveAscendancy };
  },
  { persist: true },
);
