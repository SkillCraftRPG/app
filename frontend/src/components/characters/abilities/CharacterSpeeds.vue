<script setup lang="ts">
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import CharacterSpeed from "./CharacterSpeed.vue";
import type { CharacterModel, CharacterSpeeds } from "@/types/characters";
import { calculateSpeeds } from "@/helpers/characterUtils";

const { t } = useI18n();

const props = defineProps<{
  character: CharacterModel;
}>();

const speeds = computed<CharacterSpeeds>(() => calculateSpeeds(props.character));
</script>

<template>
  <div>
    <h5>{{ t("game.speeds") }}</h5>
    <div class="mb-3 row">
      <div class="col">
        <CharacterSpeed :character="character" speed="Walk" :value="speeds.walk" />
      </div>
      <div class="col">
        <CharacterSpeed :character="character" speed="Climb" :value="speeds.climb" />
      </div>
      <div class="col">
        <CharacterSpeed :character="character" speed="Swim" :value="speeds.swim" />
      </div>
      <div class="col">
        <CharacterSpeed :character="character" speed="Fly" :value="speeds.fly" />
      </div>
      <div class="col">
        <CharacterSpeed :character="character" speed="Hover" :value="speeds.hover" />
      </div>
      <div class="col">
        <CharacterSpeed :character="character" speed="Burrow" :value="speeds.burrow" />
      </div>
    </div>
  </div>
</template>
