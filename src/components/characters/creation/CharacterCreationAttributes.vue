<template>
  <form @submit.prevent="handleSubmit(submit)">
    <h2 class="h3">{{ t("characters.attributes.title") }}</h2>
    <p class="text-body-secondary">{{ t("characters.attributes.help") }}</p>
    <section v-for="attribute in attributes" :key="attribute.key" class="mb-3">
      <div class="d-flex justify-content-between gap-2 mb-1">
        <div class="fs-5">{{ attribute.name }}</div>
        <div class="d-flex justify-content-between gap-2">
          <TarButton
            :disabled="attribute.score <= MINIMUM_SCORE"
            icon="fas fa-minus"
            outline
            size="small"
            variant="secondary"
            @click="decrease(attribute.key)"
          />
          <div class="fs-5">{{ formatSignedInteger(attribute.score, n) }}</div>
          <TarButton :disabled="attribute.score >= MAXIMUM_SCORE" icon="fas fa-plus" outline size="small" @click="increase(attribute.key)" />
        </div>
      </div>
      <div class="row">
        <div class="col">
          <table class="table table-sm">
            <tbody>
              <tr v-for="statistic in attribute.statistics" :key="statistic.key">
                <td class="w-50 text-body-secondary">{{ statistic.name }}</td>
                <td class="w-50 text-end">{{ n(statistic.value, "integer") }}</td>
              </tr>
            </tbody>
          </table>
        </div>
        <div class="col">
          <table class="table table-sm">
            <tbody>
              <tr v-for="skill in attribute.skills" :key="skill.key">
                <td class="w-50 text-body-secondary">{{ skill.name }}</td>
                <td class="w-50 text-end">{{ formatSignedInteger(skill.value, n) }}</td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </section>
    <p v-if="total" class="text-danger">{{ t("characters.attributes.invalid", { total }) }}</p>
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
import { arrayUtils } from "logitar-js";
import { computed, onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";

import TarButton from "@/components/tar/TarButton.vue";
import type { Attribute, Skill, Statistic } from "@/types/game";
import { ATTRIBUTES } from "@/types/game";
import { formatSignedInteger } from "@/utils/format";
import { calculateSkill, calculateStatisticNew, getAttributeSkills, getAttributeStatistics } from "@/utils/game";
import { useCharacterStore } from "@/stores/character";
import { useForm } from "@/forms";

const MAXIMUM_SCORE: number = +4;
const MINIMUM_SCORE: number = -2;
const character = useCharacterStore();
const { n, t } = useI18n();
const { orderBy } = arrayUtils;

defineEmits<{
  (e: "abandon"): void;
}>();

const scores = ref<Map<Attribute, number>>(new Map());

type StatisticData = {
  key: Statistic;
  name: string;
  value: number;
};
type SkillData = {
  key: Skill;
  name: string;
  value: number;
};
type AttributeData = {
  key: Attribute;
  name: string;
  score: number;
  statistics: StatisticData[];
  skills: SkillData[];
};
const attributes = computed<AttributeData[]>(() =>
  orderBy(
    ATTRIBUTES.map((attribute) => ({
      key: attribute,
      name: t(`game.attribute.options.${attribute}`),
      score: scores.value.get(attribute) ?? 0,
      statistics: orderBy(
        getAttributeStatistics(attribute).map((statistic) => ({
          key: statistic,
          name: t(`game.statistic.options.${statistic}`),
          value: calculateStatisticNew(statistic, scores.value),
        })),
        "name",
      ),
      skills: orderBy(
        getAttributeSkills(attribute).map((skill) => ({
          key: skill,
          name: t(`game.skill.options.${skill}`),
          value: calculateSkill(skill, scores.value, character.creation.talents),
        })),
        "name",
      ),
    })),
    "name",
  ),
);

const total = computed<number>(() => [...scores.value.values()].reduce((sum, score) => sum + score, 0));
const canSubmit = computed<boolean>(() => total.value === 0);

function decrease(attribute: Attribute): void {
  const score: number = scores.value.get(attribute) ?? 0;
  scores.value.set(attribute, score - 1);
}
function increase(attribute: Attribute): void {
  const score: number = scores.value.get(attribute) ?? 0;
  scores.value.set(attribute, score + 1);
}

const { handleSubmit } = useForm();
function submit(): void {
  if (canSubmit.value) {
    character.saveAttributes({
      dexterity: scores.value.get("Dexterity") ?? 0,
      health: scores.value.get("Health") ?? 0,
      intellect: scores.value.get("Intellect") ?? 0,
      senses: scores.value.get("Senses") ?? 0,
      vigor: scores.value.get("Vigor") ?? 0,
    });
  }
}

onMounted(() => {
  scores.value.clear();
  scores.value.set("Dexterity", character.creation.attributes.dexterity);
  scores.value.set("Health", character.creation.attributes.health);
  scores.value.set("Intellect", character.creation.attributes.intellect);
  scores.value.set("Senses", character.creation.attributes.senses);
  scores.value.set("Vigor", character.creation.attributes.vigor);
});
</script>
