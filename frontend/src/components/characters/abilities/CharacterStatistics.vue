<script setup lang="ts">
import { arrayUtils } from "logitar-js";
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import CharacterStatistic from "./CharacterStatistic.vue";
import type { CharacterModel } from "@/types/characters";
import type { Statistic } from "@/types/game";

type SortedStatistic = {
  text: string;
  value: Statistic;
};

const { orderBy } = arrayUtils;
const { rt, t, tm } = useI18n();

defineProps<{
  character: CharacterModel;
}>();

const statistics = computed<SortedStatistic[]>(() =>
  orderBy(
    Object.entries(tm(rt("game.statistic.options"))).map(([value, text]) => ({ text, value }) as SortedStatistic),
    "text",
  ),
);
</script>

<template>
  <div>
    <h5>{{ t("game.statistics") }}</h5>
    <div class="mb-3 row">
      <div v-for="statistic in statistics" :key="statistic.value" class="col">
        <CharacterStatistic :character="character" :statistic="statistic.value" :text="statistic.text" />
      </div>
    </div>
  </div>
</template>
