<script setup lang="ts">
import { arrayUtils } from "logitar-js";
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import CharacterStatistic from "./CharacterStatistic.vue";
import type { CharacterStatistics, CharacterAttributes, CharacterModel } from "@/types/characters";
import type { Statistic } from "@/types/game";
import { calculateStatistics } from "@/helpers/characterUtils";

type SortedStatistic = {
  text: string;
  value: Statistic;
  base: number;
  increment: number;
  total: number;
};

const { orderBy } = arrayUtils;
const { t } = useI18n();

const props = defineProps<{
  attributes: CharacterAttributes;
  character: CharacterModel;
}>();

const sortedStatistics = computed<SortedStatistic[]>(() =>
  orderBy(
    [
      {
        text: t("game.statistic.options.Constitution"),
        value: "Constitution",
        base: statistics.value.constitution.base,
        increment: statistics.value.constitution.increment,
        total: statistics.value.constitution.total,
      },
      {
        text: t("game.statistic.options.Initiative"),
        value: "Initiative",
        base: statistics.value.initiative.base,
        increment: statistics.value.initiative.increment,
        total: statistics.value.initiative.total,
      },
      {
        text: t("game.statistic.options.Learning"),
        value: "Learning",
        base: statistics.value.learning.base,
        increment: statistics.value.learning.increment,
        total: statistics.value.learning.total,
      },
      {
        text: t("game.statistic.options.Power"),
        value: "Power",
        base: statistics.value.power.base,
        increment: statistics.value.power.increment,
        total: statistics.value.power.total,
      },
      {
        text: t("game.statistic.options.Precision"),
        value: "Precision",
        base: statistics.value.precision.base,
        increment: statistics.value.precision.increment,
        total: statistics.value.precision.total,
      },
      {
        text: t("game.statistic.options.Reputation"),
        value: "Reputation",
        base: statistics.value.reputation.base,
        increment: statistics.value.reputation.increment,
        total: statistics.value.reputation.total,
      },
      {
        text: t("game.statistic.options.Strength"),
        value: "Strength",
        base: statistics.value.strength.base,
        increment: statistics.value.strength.increment,
        total: statistics.value.strength.total,
      },
    ],
    "text",
  ),
);
const statistics = computed<CharacterStatistics>(() => calculateStatistics(props.character, props.attributes));
</script>

<template>
  <div>
    <h5>{{ t("game.statistics") }}</h5>
    <div class="mb-3 row">
      <div v-for="statistic in sortedStatistics" :key="statistic.value" class="col">
        <CharacterStatistic
          :base="statistic.base"
          :character="character"
          :increment="statistic.increment"
          :statistic="statistic.value"
          :text="statistic.text"
          :total="statistic.total"
        />
      </div>
    </div>
  </div>
</template>
