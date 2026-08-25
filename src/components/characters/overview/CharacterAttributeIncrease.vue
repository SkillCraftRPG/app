<template>
  <div>
    <TarButton :disabled="!character.points.attributes" icon="fas fa-arrow-turn-up" size="small" :text="increaseText" @click="open" />
    <TarModal centered :close="t('actions.close')" fade scrollable size="large" ref="modal" :title="t('characters.attributes.increase')">
      <div class="fw-semibold mb-3" :class="{ 'text-primary': remaining, 'text-body-secondary': !remaining }">
        {{ t("characters.attributes.remaining", { remaining }) }}
      </div>
      <div v-for="attribute in attributes" :key="attribute.key" class="row">
        <div class="row mb-1">
          <div class="col fs-5">{{ attribute.name }}</div>
          <div class="col text-center">
            <TarButton :disabled="!attribute.values.spent" icon="fas fa-minus" outline size="small" variant="secondary" @click="decrease(attribute.key)" />
          </div>
          <div class="col text-center">
            {{ formatSignedInteger(attribute.values.current, n) }}
          </div>
          <div class="col text-center" :class="{ 'text-primary': attribute.values.spent }">
            {{ formatSignedInteger(attribute.values.spent, n) }}
          </div>
          <div class="col text-center fw-semibold" :class="{ 'text-primary': attribute.values.spent }">
            {{ formatSignedInteger(attribute.values.increased, n) }}
          </div>
          <div class="col text-end">
            <TarButton :disabled="!attribute.increase" icon="fas fa-plus" outline size="small" @click="increase(attribute.key)" />
          </div>
        </div>
        <div class="row">
          <div class="col-md-6">
            <div v-for="statistic in attribute.statistics" :key="statistic.key" class="row">
              <div class="col">{{ statistic.name }}</div>
              <div class="col text-center">{{ n(statistic.values.current, "integer") }}</div>
              <div class="col text-center" :class="{ 'text-primary': statistic.values.delta }">{{ formatSignedInteger(statistic.values.delta, n) }}</div>
              <div class="col text-end fw-semibold" :class="{ 'text-primary': statistic.values.delta }">{{ n(statistic.values.increased, "integer") }}</div>
            </div>
          </div>
          <div class="col-md-6 mb-3">
            <div v-for="skill in attribute.skills" :key="skill.key" class="row">
              <div class="col">{{ skill.name }}</div>
              <div class="col text-center">{{ formatSignedInteger(skill.values.current, n) }}</div>
              <div class="col text-center" :class="{ 'text-primary': skill.values.delta }">{{ formatSignedInteger(skill.values.delta, n) }}</div>
              <div class="col text-end fw-semibold" :class="{ 'text-primary': skill.values.delta }">{{ formatSignedInteger(skill.values.increased, n) }}</div>
            </div>
          </div>
        </div>
      </div>
      <template #footer>
        <TarButton icon="fas fa-ban" :text="t('actions.cancel')" variant="secondary" @click="cancel" />
        <TarButton :disabled="!spent" icon="fas fa-arrow-turn-up" :loading="isLoading" :status="t('loading')" :text="t('actions.increase')" @click="submit" />
      </template>
    </TarModal>
  </div>
</template>

<script setup lang="ts">
import { arrayUtils } from "logitar-js";
import { computed, reactive, ref } from "vue";
import { useI18n } from "vue-i18n";

import TarButton from "@/components/tar/TarButton.vue";
import TarModal from "@/components/tar/TarModal.vue";
import type { Character } from "@/types/characters";
import type { Attribute, Skill, Statistic } from "@/types/game";
import { ATTRIBUTES, SKILLS_BY_ATTRIBUTE, STATISTICS_BY_ATTRIBUTE, calculateStatistic, camelCase } from "@/utils/game";
import { formatSignedInteger } from "@/utils/format";

const MAXIMUM_SCORE: number = 5;
const { n, t } = useI18n();
const { orderBy } = arrayUtils;

const props = defineProps<{
  character: Character;
}>();

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "updated", value: Character): void;
}>();

const isLoading = ref<boolean>(false);
const modal = ref<InstanceType<typeof TarModal> | null>(null);
const progression = reactive(
  Object.fromEntries(ATTRIBUTES.map((attribute) => [camelCase(attribute), 0])) as Record<Uncapitalize<Attribute>, number>,
);

type ValueDelta = {
  current: number;
  delta: number;
  increased: number;
};
type NamedValues<TKey extends string> = {
  key: TKey;
  name: string;
  values: ValueDelta;
};
type AttributeData = {
  key: Attribute;
  name: string;
  increase: boolean;
  values: {
    current: number;
    spent: number;
    increased: number;
  };
  statistics: NamedValues<Statistic>[];
  skills: NamedValues<Skill>[];
};

const increaseText = computed<string>(() => `${t("actions.increase")} (${formatSignedInteger(props.character.points.attributes, n)})`);
const spent = computed<number>(() => ATTRIBUTES.reduce((sum, attribute) => sum + progression[camelCase(attribute)], 0));
const remaining = computed<number>(() => props.character.points.attributes - spent.value);

const attributes = computed<AttributeData[]>(() => {
  const totals = Object.fromEntries(
    ATTRIBUTES.map((attribute) => {
      const key = camelCase(attribute);
      return [key, props.character.attributes[key].total + progression[key]];
    }),
  ) as Record<Uncapitalize<Attribute>, number>;

  return orderBy(
    ATTRIBUTES.map((attribute) => {
      const key = camelCase(attribute);
      const current = props.character.attributes[key];
      const spentPoints = progression[key];
      return {
        key: attribute,
        name: t(`game.attribute.options.${attribute}`),
        increase: remaining.value > 0 && current.starting + current.progression + spentPoints < MAXIMUM_SCORE,
        values: {
          current: current.total,
          spent: spentPoints,
          increased: current.total + spentPoints,
        },
        statistics: orderBy(
          STATISTICS_BY_ATTRIBUTE[attribute].map((statistic) => {
            const statisticKey = camelCase(statistic);
            const values = props.character.statistics[statisticKey];
            const increased = calculateStatistic(statistic, props.character.level, totals) + values.modifiers;
            return {
              key: statistic,
              name: t(`game.statistic.options.${statistic}`),
              values: {
                current: values.total,
                delta: increased - values.total,
                increased,
              },
            };
          }),
          "name",
        ),
        skills: orderBy(
          SKILLS_BY_ATTRIBUTE[attribute].map((skill) => {
            const skillKey = camelCase(skill);
            const values = props.character.skills[skillKey];
            return {
              key: skill,
              name: t(`game.skill.options.${skill}`),
              values: {
                current: values.total,
                delta: spentPoints,
                increased: values.total + spentPoints,
              },
            };
          }),
          "name",
        ),
      };
    }),
    "name",
  );
});

function decrease(attribute: Attribute): void {
  progression[camelCase(attribute)]--;
}
function increase(attribute: Attribute): void {
  progression[camelCase(attribute)]++;
}

function reset(): void {
  for (const attribute of ATTRIBUTES) {
    progression[camelCase(attribute)] = 0;
  }
}

async function submit(): Promise<void> {
  if (!isLoading.value) {
    isLoading.value = true;
    try {
      console.log("Submitting!"); // TODO(fpion): implement
    } catch (e: unknown) {
      emit("error", e);
    } finally {
      isLoading.value = false;
    }
  }
}

function cancel(): void {
  reset();
  modal.value?.hide();
}

function open(): void {
  modal.value?.show();
}
</script>
