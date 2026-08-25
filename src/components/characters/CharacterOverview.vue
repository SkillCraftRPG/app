<template>
  <div>
    <div class="row">
      <div class="col-md-4 mb-3">
        <CharacterLineage :lineage="character.lineage" />
      </div>
      <div class="col-md-4 mb-3">
        <CharacterCaste :caste="character.caste" />
      </div>
      <div class="col-md-4 mb-3">
        <CharacterEducation :education="character.education" />
      </div>
      <div class="col-md-4 mb-3">
        <CharacterExperience :character="character" @error="$emit('error', $event)" @updated="$emit('updated', $event)" />
      </div>
      <div class="col-md-4 mb-3">
        <CharacterVitality :character="character" @error="$emit('error', $event)" @updated="$emit('updated', $event)" />
      </div>
      <div class="col-md-4 mb-3">
        <CharacterStamina :character="character" @error="$emit('error', $event)" @updated="$emit('updated', $event)" />
      </div>
      <div class="col-md-4 mb-3">
        <CharacterHope :character="character" @error="$emit('error', $event)" @updated="$emit('updated', $event)" />
      </div>
      <div class="col-md-4 mb-3">
        <CharacterAlcohol :character="character" @error="$emit('error', $event)" @updated="$emit('updated', $event)" />
      </div>
      <div class="col-md-4 mb-3">
        <CharacterIntoxication :character="character" @error="$emit('error', $event)" @updated="$emit('updated', $event)" />
      </div>
    </div>
    <CharacterConditions v-if="hasConditions" :character="character" class="mb-3" />
    <CharacterAttributes :character="character" @error="$emit('error', $event)" @updated="$emit('updated', $event)" />
    <CharacterStatistics :character="character" />
    <CharacterSkills :character="character" />
    <CharacterSpeeds :character="character" />
    <CharacterCustomizations :character="character" @error="$emit('error', $event)" @updated="$emit('updated', $event)" />
    <CharacterLanguages :character="character" @error="$emit('error', $event)" @updated="$emit('updated', $event)" />
    <!-- TODO(fpion): Specializations -->
  </div>
</template>

<script setup lang="ts">
import { computed } from "vue";

import CharacterAlcohol from "./status/CharacterAlcohol.vue";
import CharacterAttributes from "./overview/CharacterAttributes.vue";
import CharacterCaste from "./overview/CharacterCaste.vue";
import CharacterConditions from "./status/CharacterConditions.vue";
import CharacterCustomizations from "./customizations/CharacterCustomizations.vue";
import CharacterEducation from "./overview/CharacterEducation.vue";
import CharacterExperience from "./status/CharacterExperience.vue";
import CharacterHope from "./status/CharacterHope.vue";
import CharacterIntoxication from "./status/CharacterIntoxication.vue";
import CharacterLanguages from "./languages/CharacterLanguages.vue";
import CharacterLineage from "./overview/CharacterLineage.vue";
import CharacterSkills from "./overview/CharacterSkills.vue";
import CharacterSpeeds from "./overview/CharacterSpeeds.vue";
import CharacterStamina from "./status/CharacterStamina.vue";
import CharacterStatistics from "./overview/CharacterStatistics.vue";
import CharacterVitality from "./status/CharacterVitality.vue";
import type { Character } from "@/types/characters";

defineProps<{
  character: Character;
}>();

defineEmits<{
  (e: "error", value: unknown): void;
  (e: "updated", value: Character): void;
}>();

const hasConditions = computed<boolean>(() => false); // TODO(fpion): implement
</script>
