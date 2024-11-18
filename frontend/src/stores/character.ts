import { defineStore } from "pinia";
import { ref } from "vue";

import type { CharacterCreation } from "@/types/characters";

export const useCharacterStore = defineStore("character", () => {
  const creation = ref<CharacterCreation>();

  return { creation };
}); // TODO(fpion): unit tests
