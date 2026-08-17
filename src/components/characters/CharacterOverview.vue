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
      <div class="col-md-4">
        <CharacterExperience class="mb-3" :character="character" />
      </div>
      <div class="col-md-4">
        <CharacterVitality class="mb-3" :character="character" />
      </div>
      <div class="col-md-4">
        <CharacterStamina class="mb-3" :character="character" />
      </div>
      <div class="col-md-4">
        <CharacterHope class="mb-3" :character="character" />
      </div>
      <div class="col-md-4">
        <CharacterAlcohol class="mb-3" :character="character" />
      </div>
      <div class="col-md-4">
        <CharacterIntoxication class="mb-3" :character="character" />
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
    <template v-if="character.customizations.length">
      <div class="fs-5 mb-1">{{ t("customizations.title") }}</div>
      <CharacterCustomizations :customizations="character.customizations" />
    </template>
    <template v-if="hasLanguages">
      <div class="fs-5 mb-1">{{ t("languages.title") }}</div>
      <CharacterLanguages :languages="character.languages" :lineage="character.lineage" />
    </template>
    <!-- TODO(fpion): Specializations -->
  </div>
</template>

<script setup lang="ts">
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import CharacterAlcohol from "./CharacterAlcohol.vue";
import CharacterAttributes from "./CharacterAttributes.vue";
import CharacterCaste from "./CharacterCaste.vue";
import CharacterConditions from "./CharacterConditions.vue";
import CharacterCustomizations from "./CharacterCustomizations.vue";
import CharacterEducation from "./CharacterEducation.vue";
import CharacterExperience from "./CharacterExperience.vue";
import CharacterHope from "./CharacterHope.vue";
import CharacterIntoxication from "./CharacterIntoxication.vue";
import CharacterLanguages from "./CharacterLanguages.vue";
import CharacterLineage from "./CharacterLineage.vue";
import CharacterSkills from "./CharacterSkills.vue";
import CharacterSpeeds from "./CharacterSpeeds.vue";
import CharacterStamina from "./CharacterStamina.vue";
import CharacterStatistics from "./CharacterStatistics.vue";
import CharacterVitality from "./CharacterVitality.vue";
import type { Character } from "@/types/characters";

const { t } = useI18n();

const props = defineProps<{
  character: Character;
}>();

const hasConditions = computed<boolean>(() => false); // TODO(fpion): implement
const hasLanguages = computed<boolean>(() =>
  Boolean(props.character.languages.length || props.character.lineage.languages.granted.length || props.character.lineage.parent?.languages.granted.length),
);
</script>
