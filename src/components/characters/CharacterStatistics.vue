<template>
  <div class="row">
    <div v-for="score in scores" :key="score.key" class="col-6 col-md-4 col-lg-fifth">
      <CharacterStatistic :attribute="score.attribute" class="mb-3" :statistic="score.statistic" :value="score.value" />
    </div>
  </div>
</template>

<script setup lang="ts">
import { arrayUtils } from "logitar-js";
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import CharacterStatistic from "./CharacterStatistic.vue";
import type { CharacterStatistics } from "@/types/characters";

const { orderBy } = arrayUtils;
const { t } = useI18n();

const props = defineProps<{
  statistics: CharacterStatistics;
}>();

type StatisticScore = {
  key: string;
  attribute: string;
  statistic: string;
  value: number;
};
const scores = computed<StatisticScore[]>(() =>
  orderBy(
    [
      {
        key: "dodge",
        attribute: t("game.attribute.options.Dexterity"),
        statistic: t("game.statistic.options.Dodge"),
        value: props.statistics.dodge.total,
      },
      {
        key: "initiative",
        attribute: t("game.attribute.options.Senses"),
        statistic: t("game.statistic.options.Initiative"),
        value: props.statistics.initiative.total,
      },
      {
        key: "learning",
        attribute: t("game.attribute.options.Intellect"),
        statistic: t("game.statistic.options.Learning"),
        value: props.statistics.learning.total,
      },
      {
        key: "load",
        attribute: t("game.attribute.options.Vigor"),
        statistic: t("game.statistic.options.Load"),
        value: props.statistics.load.total,
      },
      {
        key: "power",
        attribute: t("game.attribute.options.Senses"),
        statistic: t("game.statistic.options.Power"),
        value: props.statistics.power.total,
      },
      {
        key: "precision",
        attribute: t("game.attribute.options.Dexterity"),
        statistic: t("game.statistic.options.Precision"),
        value: props.statistics.precision.total,
      },
      {
        key: "stamina",
        attribute: t("game.attribute.options.Health"),
        statistic: t("game.statistic.options.Stamina"),
        value: props.statistics.stamina.total,
      },
      {
        key: "stratagem",
        attribute: t("game.attribute.options.Intellect"),
        statistic: t("game.statistic.options.Stratagem"),
        value: props.statistics.stratagem.total,
      },
      {
        key: "strength",
        attribute: t("game.attribute.options.Vigor"),
        statistic: t("game.statistic.options.Strength"),
        value: props.statistics.strength.total,
      },
      {
        key: "vitality",
        attribute: t("game.attribute.options.Health"),
        statistic: t("game.statistic.options.Vitality"),
        value: props.statistics.vitality.total,
      },
    ],
    "statistic",
  ),
);
</script>
