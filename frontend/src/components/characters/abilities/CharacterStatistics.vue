<script setup lang="ts">
import { arrayUtils } from "logitar-js";
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import CharacterStatistic from "./CharacterStatistic.vue";
import type { CharacterStatistic as CharacterStatisticT, CharacterStatistics, CharacterAttributes, CharacterModel } from "@/types/characters";
import type { Statistic } from "@/types/game";
import { calculateStatistics } from "@/helpers/characterUtils";

type SortedStatistic = {
  text: string;
  value: Statistic;
}; // TODO(fpion): refactor to include CharacterStatisticT

const { orderBy } = arrayUtils;
const { rt, t, tm } = useI18n();

const props = defineProps<{
  attributes: CharacterAttributes;
  character: CharacterModel;
}>();

const sortedStatistics = computed<SortedStatistic[]>(() =>
  orderBy(
    Object.entries(tm(rt("game.statistic.options"))).map(([value, text]) => ({ text, value }) as SortedStatistic),
    "text",
  ),
);
const statistics = computed<CharacterStatistics>(() => calculateStatistics(props.character, props.attributes));

function getStatistic(statistic: Statistic): CharacterStatisticT {
  switch (statistic) {
    case "Constitution":
      return statistics.value.constitution;
    case "Initiative":
      return statistics.value.initiative;
    case "Learning":
      return statistics.value.learning;
    case "Power":
      return statistics.value.power;
    case "Precision":
      return statistics.value.precision;
    case "Reputation":
      return statistics.value.reputation;
    case "Strength":
      return statistics.value.strength;
    default:
      throw new Error(`The statistic '${statistic}' is not supported.`);
  }
}
function getBase(statistic: Statistic): number {
  return getStatistic(statistic).base;
}
function getIncrement(statistic: Statistic): number {
  return getStatistic(statistic).increment;
}
function getTotal(statistic: Statistic): number {
  return getStatistic(statistic).total;
}
</script>

<template>
  <div>
    <h5>{{ t("game.statistics") }}</h5>
    <div class="mb-3 row">
      <div v-for="statistic in sortedStatistics" :key="statistic.value" class="col">
        <CharacterStatistic
          :base="getBase(statistic.value)"
          :character="character"
          :increment="getIncrement(statistic.value)"
          :statistic="statistic.value"
          :text="statistic.text"
          :total="getTotal(statistic.value)"
        />
      </div>
    </div>
  </div>
</template>
