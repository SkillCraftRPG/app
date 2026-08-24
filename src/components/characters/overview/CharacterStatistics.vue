<template>
  <div class="row">
    <div v-for="statistic in statistics" :key="statistic.key" class="col-6 col-md-4 col-lg-fifth">
      <CharacterStatistic class="mb-3" :character="character" :name="statistic.name" :statistic="statistic.key" />
    </div>
  </div>
</template>

<script setup lang="ts">
import { arrayUtils } from "logitar-js";
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import CharacterStatistic from "./CharacterStatistic.vue";
import type { Character } from "@/types/characters";
import type { Statistic } from "@/types/game";
import { STATISTICS } from "@/utils/game";

const { orderBy } = arrayUtils;
const { t } = useI18n();

defineProps<{
  character: Character;
}>();

type StatisticData = {
  key: Statistic;
  name: string;
};
const statistics = computed<StatisticData[]>(() =>
  orderBy(
    STATISTICS.map((key) => ({ key, name: t(`game.statistic.options.${key}`) })),
    "name",
  ),
);
</script>
