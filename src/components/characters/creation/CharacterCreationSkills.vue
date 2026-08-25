<template>
  <form @submit.prevent="handleSubmit(submit)">
    <h2 class="h3">{{ t("characters.skills.title") }}</h2>
    <p class="text-body-secondary">{{ t("characters.skills.help") }}</p>
    <div class="row text-center mb-3">
      <div class="col">
        <div class="fw-bold">{{ t("game.statistic.options.Learning") }}</div>
        <div>{{ n(learning, "integer") }}</div>
      </div>
      <div class="col">
        <div class="fw-bold">{{ t("characters.skills.spent") }}</div>
        <div>{{ n(spent, "integer") }}</div>
      </div>
      <div class="col" :class="{ 'text-danger': remaining < 0 }">
        <div class="fw-bold">{{ t("characters.skills.remaining") }}</div>
        <div>{{ n(remaining, "integer") }}</div>
      </div>
    </div>
    <TarCard v-for="attribute in attributes" :key="attribute.key" class="mb-3">
      <div class="d-flex justify-content-between gap-2 mb-2">
        <div class="fs-5">{{ attribute.name }}</div>
        <div v-if="typeof attribute.score === 'number'" class="fs-5">{{ formatSignedInteger(attribute.score, n) }}</div>
      </div>
      <div class="row g-2">
        <div v-for="skill in attribute.skills" :key="skill.key" class="col-sm-6 col-md-4 col-lg-3 col-xl-fifth">
          <div class="border rounded p-2">
            <div class="d-flex justify-content-between gap-2 mb-1">
              <div class="text-body-secondary">{{ skill.name }}</div>
              <div class="fw-semibold">{{ formatSignedInteger(skill.total, n) }}</div>
            </div>
            <div class="d-flex justify-content-between align-items-end gap-2">
              <div>
                <TarButton :disabled="!skill.canDecrease" icon="fas fa-minus" outline size="small" variant="secondary" @click="decrease(skill.key)" />
                <span class="mx-2">{{ n(skill.talents + skill.rank, "integer") }}</span>
                <TarButton :disabled="!skill.canIncrease" icon="fas fa-plus" outline size="small" @click="increase(skill.key)" />
              </div>
              <div>
                <TarBadge v-if="skill.talents" pill variant="secondary">
                  <font-awesome-icon icon="fas fa-graduation-cap" aria-hidden="true" />&nbsp;{{ t("characters.skills.trained") }}
                </TarBadge>
              </div>
            </div>
          </div>
        </div>
      </div>
    </TarCard>
    <p v-if="!canSubmit" class="text-danger">{{ t("characters.skills.invalid", { learning, spent }) }}</p>
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

import TarBadge from "@/components/tar/TarBadge.vue";
import TarButton from "@/components/tar/TarButton.vue";
import TarCard from "@/components/tar/TarCard.vue";
import type { Attribute, Skill } from "@/types/game";
import type { SkillRankPayload } from "@/types/characters";
import { ATTRIBUTES } from "@/types/game";
import { formatSignedInteger } from "@/utils/format";
import { calculateSkill, calculateStatisticNew, getAttributeSkills } from "@/utils/game";
import { useCharacterStore } from "@/stores/character";
import { useForm } from "@/forms";

const MAXIMUM_VALUE: number = 2;
const character = useCharacterStore();
const { orderBy } = arrayUtils;
const { n, t } = useI18n();

defineEmits<{
  (e: "abandon"): void;
}>();

const ranks = ref<Map<Skill, number>>(new Map());

const attributeScores = computed<Map<Attribute, number>>(
  () =>
    new Map([
      ["Dexterity", character.creation.attributes.dexterity],
      ["Health", character.creation.attributes.health],
      ["Intellect", character.creation.attributes.intellect],
      ["Senses", character.creation.attributes.senses],
      ["Vigor", character.creation.attributes.vigor],
    ]),
);
const learning = computed<number>(() => calculateStatisticNew("Learning", attributeScores.value));
const spent = computed<number>(() => [...ranks.value.values()].reduce((sum, rank) => sum + rank, 0));
const remaining = computed<number>(() => learning.value - spent.value);
const canSubmit = computed<boolean>(() => remaining.value >= 0);

const talents = computed<Map<Skill, number>>(() => {
  const talents: Map<Skill, number> = new Map();
  character.creation.talents.forEach((acquired) => {
    if (acquired.talent.skill) {
      const count: number = talents.get(acquired.talent.skill) ?? 0;
      talents.set(acquired.talent.skill, count + 1);
    }
  });
  return talents;
});

type SkillData = {
  key: Skill;
  name: string;
  talents: number;
  rank: number;
  total: number;
  canDecrease: boolean;
  canIncrease: boolean;
};
type AttributeData = {
  key: Attribute | "Social";
  name: string;
  score?: number;
  skills: SkillData[];
};
const attributes = computed<AttributeData[]>(() => {
  const attributes: AttributeData[] = orderBy(
    ATTRIBUTES.map((attribute) => ({
      key: attribute,
      name: t(`game.attribute.options.${attribute}`),
      score: attributeScores.value.get(attribute),
      skills: orderBy(
        getAttributeSkills(attribute).map((skill) => {
          const talentCount: number = talents.value.get(skill) ?? 0;
          const rank: number = ranks.value.get(skill) ?? 0;
          return {
            key: skill,
            name: t(`game.skill.options.${skill}`),
            talents: talentCount,
            rank,
            total: calculateSkill(skill, attributeScores.value, character.creation.talents, rank),
            canDecrease: rank > 0,
            canIncrease: talentCount + rank < MAXIMUM_VALUE,
          };
        }),
        "name",
      ),
    })),
    "name",
  );
  attributes.push({
    key: "Social",
    name: t("characters.social"),
    skills: orderBy(
      getAttributeSkills("Social").map((skill) => {
        const talentCount: number = talents.value.get(skill) ?? 0;
        const rank: number = ranks.value.get(skill) ?? 0;
        return {
          key: skill,
          name: t(`game.skill.options.${skill}`),
          talents: talentCount,
          rank,
          total: calculateSkill(skill, attributeScores.value, character.creation.talents, rank),
          canDecrease: rank > 0,
          canIncrease: talentCount + rank < MAXIMUM_VALUE,
        };
      }),
    ),
  });
  return attributes;
});

function decrease(skill: Skill): void {
  const rank: number = ranks.value.get(skill) ?? 0;
  ranks.value.set(skill, rank - 1);
}
function increase(skill: Skill): void {
  const rank: number = ranks.value.get(skill) ?? 0;
  ranks.value.set(skill, rank + 1);
}

const { handleSubmit } = useForm();
function submit(): void {
  if (canSubmit.value) {
    const skillRanks: SkillRankPayload[] = [];
    for (const [skill, rank] of ranks.value) {
      if (rank) {
        skillRanks.push({ skill, rank });
      }
    }
    character.saveSkills(skillRanks);
  }
}

onMounted(() => character.creation.skills.forEach((item) => ranks.value.set(item.skill, item.rank)));
</script>
