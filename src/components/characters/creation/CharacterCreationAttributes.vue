<template>
  <form @submit.prevent="handleSubmit(submit)">
    <h2 class="h3">{{ t("characters.attributes.title") }}</h2>
    <p class="text-body-secondary">{{ t("characters.attributes.help") }}</p>
    <div v-for="attribute in attributes" :key="attribute.key" class="row text-center mb-3">
      <div class="col">
        <InputField
          :id="attribute.key"
          :label="attribute.name"
          :min="-2"
          :max="4"
          :model-value="attribute.score.toString()"
          required
          step="1"
          type="number"
          @update:model-value="updateScore(attribute.key, parseNumber($event) ?? 0)"
        />
      </div>
      <div v-for="statistic in attribute.statistics" :key="statistic.key" class="col">
        <div class="fw-bold">{{ statistic.name }}</div>
        <div>{{ n(statistic.value, "integer") }}</div>
      </div>
    </div>
    <p v-if="!canSubmit" class="text-danger">{{ t("characters.attributes.invalid", { total }) }}</p>
    <div class="d-flex justify-content-between">
      <div class="d-flex gap-2">
        <TarButton icon="fas fa-xmark" outline :text="t('actions.abandon')" variant="danger" @click="$emit('abandon')" />
        <TarButton icon="fas fa-arrow-left" outline :text="t('actions.previous')" variant="secondary" @click="character.goBack" />
      </div>
      <TarButton :disabled="!canSubmit" icon="fas fa-arrow-right" :text="t('actions.next')" type="submit" />
    </div>
  </form>
</template>

<script setup lang="ts">
import { arrayUtils, parsingUtils } from "logitar-js";
import { computed, onMounted, reactive } from "vue";
import { useI18n } from "vue-i18n";

import InputField from "@/components/forms/InputField.vue";
import TarButton from "@/components/tar/TarButton.vue";
import type { Attribute, Statistic } from "@/types/game";
import type { StartingAttributes } from "@/types/characters";
import { ATTRIBUTES, STATISTICS_BY_ATTRIBUTE, calculateStatistic, camelCase } from "@/utils/game";
import { useCharacterStore } from "@/stores/character";
import { useForm } from "@/forms";

const character = useCharacterStore();
const { n, t } = useI18n();
const { orderBy } = arrayUtils;
const { parseNumber } = parsingUtils;

defineEmits<{
  (e: "abandon"): void;
}>();

const scores = reactive<StartingAttributes>(Object.fromEntries(ATTRIBUTES.map((key) => [camelCase(key), 0])) as StartingAttributes);

type StatisticData = {
  key: Statistic;
  name: string;
  value: number;
};
type AttributeData = {
  key: Attribute;
  name: string;
  score: number;
  statistics: StatisticData[];
};
const attributes = computed<AttributeData[]>(() =>
  orderBy(
    ATTRIBUTES.map((key) => ({
      key,
      name: t(`game.attribute.options.${key}`),
      score: scores[camelCase(key)],
      statistics: orderBy(
        STATISTICS_BY_ATTRIBUTE[key].map((statistic) => ({
          key: statistic,
          name: t(`game.statistic.options.${statistic}`),
          value: calculateStatistic(statistic, scores),
        })),
        "name",
      ),
    })),
    "name",
  ),
);
const total = computed<number>(() => attributes.value.reduce((sum, attribute) => sum + attribute.score, 0));
const canSubmit = computed<boolean>(() => !total.value);

function updateScore(attribute: Attribute, value: number): void {
  scores[camelCase(attribute)] = value;
}

const { handleSubmit } = useForm();
function submit(): void {
  if (canSubmit.value) {
    character.saveAttributes({ ...scores });
  }
}

onMounted(() => {
  Object.assign(scores, character.creation.attributes);
});
</script>
