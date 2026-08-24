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
        <CharacterExperience :character="character" />
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
    <div class="fs-5 mb-1">{{ t("characters.attributes.title") }}</div>
    <CharacterAttributes :character="character" />
    <div class="fs-5 mb-1">{{ t("characters.statistics.title") }}</div>
    <CharacterStatistics :character="character" />
    <div class="fs-5 mb-1">{{ t("characters.skills.title") }}</div>
    <CharacterSkills :character="character" />
    <div class="fs-5 mb-1">{{ t("lineages.physical.speeds.lead") }}</div>
    <CharacterSpeeds :character="character" />
    <CharacterCustomizations :character="character" @error="$emit('error', $event)" @updated="$emit('updated', $event)" />
    <CharacterLanguages :character="character" @error="$emit('error', $event)" @updated="$emit('updated', $event)" />
    <!-- TODO(fpion): Specializations -->
  </div>
</template>

<script setup lang="ts">
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import CharacterAlcohol from "./status/CharacterAlcohol.vue";
import CharacterAttributes from "./CharacterAttributes.vue";
import CharacterCaste from "./CharacterCaste.vue";
import CharacterConditions from "./CharacterConditions.vue";
import CharacterCustomizations from "./customizations/CharacterCustomizations.vue";
import CharacterEducation from "./CharacterEducation.vue";
import CharacterExperience from "./CharacterExperience.vue";
import CharacterHope from "./status/CharacterHope.vue";
import CharacterIntoxication from "./status/CharacterIntoxication.vue";
import CharacterLanguages from "./languages/CharacterLanguages.vue";
import CharacterLineage from "./CharacterLineage.vue";
import CharacterSkills from "./CharacterSkills.vue";
import CharacterSpeeds from "./CharacterSpeeds.vue";
import CharacterStamina from "./status/CharacterStamina.vue";
import CharacterStatistics from "./CharacterStatistics.vue";
import CharacterVitality from "./status/CharacterVitality.vue";
import type { Character } from "@/types/characters";

const { t } = useI18n();

defineProps<{
  character: Character;
}>();

defineEmits<{
  (e: "error", value: unknown): void;
  (e: "updated", value: Character): void;
}>();

const hasConditions = computed<boolean>(() => false); // TODO(fpion): implement
</script>
