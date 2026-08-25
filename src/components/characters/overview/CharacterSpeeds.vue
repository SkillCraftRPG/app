<template>
  <div>
    <div class="fs-5 mb-1">{{ t("lineages.physical.speeds.lead") }}</div>
    <div class="row">
      <div v-for="speed in speeds" :key="speed.key" class="col-6 col-md-4 col-lg-fifth">
        <CharacterSpeed class="mb-3" :character="character" :kind="speed.key" :name="speed.name" />
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import CharacterSpeed from "./CharacterSpeed.vue";
import type { Character } from "@/types/characters";
import type { SpeedKind } from "@/types/game";
import { SPEED_KINDS } from "@/utils/game";

const { t } = useI18n();

const props = defineProps<{
  character: Character;
}>();

type SpeedData = {
  key: SpeedKind;
  name: string;
};
const speeds = computed<SpeedData[]>(() =>
  SPEED_KINDS.map((key) => ({
    key,
    name: t(key === "Fly" && props.character.speeds.hover ? "game.speed.hover" : `game.speed.kind.options.${key}`),
  })),
);
</script>
