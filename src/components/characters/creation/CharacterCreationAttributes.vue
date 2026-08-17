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
import { computed, onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";

import InputField from "@/components/forms/InputField.vue";
import TarButton from "@/components/tar/TarButton.vue";
import { useCharacterStore } from "@/stores/character";
import { useForm } from "@/forms";

const character = useCharacterStore();
const { n, t } = useI18n();
const { orderBy } = arrayUtils;
const { parseNumber } = parsingUtils;

defineEmits<{
  (e: "abandon"): void;
}>();

const dexterity = ref<number>(0);
const health = ref<number>(0);
const intellect = ref<number>(0);
const senses = ref<number>(0);
const vigor = ref<number>(0);

const dodge = computed<number>(() => 10 + dexterity.value);
const initiative = computed<number>(() => 2 * senses.value);
const learning = computed<number>(() => Math.max(5 + intellect.value, 5));
const load = computed<number>(() => 10 * (5 + vigor.value));
const power = computed<number>(() => 5 + senses.value * 2);
const precision = computed<number>(() => 5 + dexterity.value * 2);
const stamina = computed<number>(() => 25 + health.value * 5);
const stratagem = computed<number>(() => 5 + intellect.value * 2);
const strength = computed<number>(() => 5 + vigor.value * 2);
const vitality = computed<number>(() => 25 + health.value * 5);

type Statistic = {
  key: string;
  name: string;
  value: number;
};
type Attribute = {
  key: string;
  name: string;
  score: number;
  statistics: Statistic[];
};
const attributes = computed<Attribute[]>(() =>
  orderBy(
    [
      {
        key: "dexterity",
        name: t("game.attribute.options.Dexterity"),
        score: dexterity.value,
        statistics: orderBy(
          [
            {
              key: "dodge",
              name: t("game.statistic.options.Dodge"),
              value: dodge.value,
            },
            {
              key: "precision",
              name: t("game.statistic.options.Precision"),
              value: precision.value,
            },
          ],
          "name",
        ),
      },
      {
        key: "health",
        name: t("game.attribute.options.Health"),
        score: health.value,
        statistics: orderBy(
          [
            {
              key: "stamina",
              name: t("game.statistic.options.Stamina"),
              value: stamina.value,
            },
            {
              key: "vitality",
              name: t("game.statistic.options.Vitality"),
              value: vitality.value,
            },
          ],
          "name",
        ),
      },
      {
        key: "intellect",
        name: t("game.attribute.options.Intellect"),
        score: intellect.value,
        statistics: orderBy(
          [
            {
              key: "learning",
              name: t("game.statistic.options.Learning"),
              value: learning.value,
            },
            {
              key: "stratagem",
              name: t("game.statistic.options.Stratagem"),
              value: stratagem.value,
            },
          ],
          "name",
        ),
      },
      {
        key: "senses",
        name: t("game.attribute.options.Senses"),
        score: senses.value,
        statistics: orderBy(
          [
            {
              key: "initiative",
              name: t("game.statistic.options.Initiative"),
              value: initiative.value,
            },
            {
              key: "power",
              name: t("game.statistic.options.Power"),
              value: power.value,
            },
          ],
          "name",
        ),
      },
      {
        key: "vigor",
        name: t("game.attribute.options.Vigor"),
        score: vigor.value,
        statistics: orderBy(
          [
            {
              key: "load",
              name: t("game.statistic.options.Load"),
              value: load.value,
            },
            {
              key: "strength",
              name: t("game.statistic.options.Strength"),
              value: strength.value,
            },
          ],
          "name",
        ),
      },
    ],
    "name",
  ),
);
const total = computed<number>(() => attributes.value.reduce((sum, attribute) => sum + attribute.score, 0));
const canSubmit = computed<boolean>(() => !total.value);

function updateScore(attribute: string, value: number): void {
  switch (attribute) {
    case "dexterity":
      dexterity.value = value;
      break;
    case "health":
      health.value = value;
      break;
    case "intellect":
      intellect.value = value;
      break;
    case "senses":
      senses.value = value;
      break;
    case "vigor":
      vigor.value = value;
      break;
  }
}

const { handleSubmit } = useForm();
function submit(): void {
  if (canSubmit.value) {
    character.saveAttributes({
      dexterity: dexterity.value,
      health: health.value,
      intellect: intellect.value,
      senses: senses.value,
      vigor: vigor.value,
    });
  }
}

onMounted(() => {
  dexterity.value = character.creation.attributes.dexterity;
  health.value = character.creation.attributes.health;
  intellect.value = character.creation.attributes.intellect;
  senses.value = character.creation.attributes.senses;
  vigor.value = character.creation.attributes.vigor;
});
</script>
