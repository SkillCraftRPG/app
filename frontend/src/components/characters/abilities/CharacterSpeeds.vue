<script setup lang="ts">
import { useI18n } from "vue-i18n";

import CharacterSpeed from "./CharacterSpeed.vue";
import type { CharacterModel } from "@/types/characters";

const { t } = useI18n();

defineProps<{
  character: CharacterModel;
}>();
</script>

<template>
  <div>
    <h5>{{ t("game.speeds") }}</h5>
    <div class="mb-3 row">
      <div class="col">
        <CharacterSpeed :character="character" speed="Walk" />
      </div>
      <div class="col">
        <CharacterSpeed :character="character" speed="Climb" />
      </div>
      <div class="col">
        <CharacterSpeed :character="character" speed="Swim" />
      </div>
      <div class="col">
        <CharacterSpeed :character="character" speed="Fly" />
      </div>
      <div class="col">
        <CharacterSpeed :character="character" speed="Hover" />
      </div>
      <div class="col">
        <CharacterSpeed :character="character" speed="Burrow" />
      </div>
    </div>
  </div>
</template>
