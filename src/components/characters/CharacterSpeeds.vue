<template>
  <div class="row">
    <div v-for="score in scores" :key="score.key" class="col-6 col-md-4 col-lg-fifth">
      <CharacterSpeed class="mb-3" :speed="score.speed" :value="score.value" />
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import CharacterSpeed from "./CharacterSpeed.vue";
import type { CharacterSpeeds } from "@/types/characters";

const { t } = useI18n();

const props = defineProps<{
  speeds: CharacterSpeeds;
}>();

type SpeedScore = {
  key: string;
  speed: string;
  value: number;
};
const scores = computed<SpeedScore[]>(() => [
  {
    key: "walk",
    speed: t("game.speed.kind.options.Walk"),
    value: props.speeds.walk.total,
  },
  {
    key: "climb",
    speed: t("game.speed.kind.options.Climb"),
    value: props.speeds.climb.total,
  },
  {
    key: "swim",
    speed: t("game.speed.kind.options.Swim"),
    value: props.speeds.swim.total,
  },
  {
    key: props.speeds.hover ? "hover" : "fly",
    speed: t(props.speeds.hover ? "game.speed.hover" : "game.speed.kind.options.Fly"),
    value: props.speeds.fly.total,
  },
  {
    key: "burrow",
    speed: t("game.speed.kind.options.Burrow"),
    value: props.speeds.burrow.total,
  },
]);
</script>
