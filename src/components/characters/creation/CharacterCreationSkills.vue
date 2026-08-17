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
      <div class="col">
        <div class="fw-bold">{{ t("characters.skills.remaining") }}</div>
        <div>{{ n(remaining, "integer") }}</div>
      </div>
    </div>
    <TarCard v-for="attribute in attributes" :key="attribute.key" class="mb-3">
      <template #title-override>
        <h5 class="card-title d-flex justify-content-between align-items-start gap-3 w-100">
          <span>{{ attribute.name }}</span>
          <TarBadge v-if="attribute.score" pill variant="secondary">{{ attribute.score }}</TarBadge>
        </h5>
      </template>
      <div class="row">
        <div v-for="skill in attribute.skills" :key="skill.key" class="col-6 col-sm-4 col-lg-3 col-xl-fifth">
          <InputField
            class="mb-3"
            :disabled="skill.talents >= 2"
            :id="skill.key"
            :label="skill.name"
            :min="skill.talents >= 2 ? 2 : skill.talents"
            :max="2"
            :model-value="skill.rank.toString()"
            step="1"
            type="number"
            @update:model-value="updateRank(skill.key, parseNumber($event) ?? 0)"
          />
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
import { arrayUtils, parsingUtils } from "logitar-js";
import { computed, onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";

import InputField from "@/components/forms/InputField.vue";
import TarBadge from "@/components/tar/TarBadge.vue";
import TarButton from "@/components/tar/TarButton.vue";
import TarCard from "@/components/tar/TarCard.vue";
import type { Skill } from "@/types/game";
import type { SkillRankPayload } from "@/types/characters";
import { useCharacterStore } from "@/stores/character";
import { useForm } from "@/forms";

const character = useCharacterStore();
const { orderBy } = arrayUtils;
const { parseNumber } = parsingUtils;
const { n, t } = useI18n();

defineEmits<{
  (e: "abandon"): void;
}>();

const ranks = ref<Map<Skill, number>>(new Map());

const skills = computed<Map<string, Skill[]>>(
  () =>
    new Map([
      ["Dexterity", ["Acrobatics", "Crafting", "Orientation", "Stealth", "Thievery"]],
      ["Health", ["Discipline", "Resistance"]],
      ["Intellect", ["Investigation", "Knowledge", "Linguistics", "Medicine"]],
      ["Senses", ["Insight", "Occultism", "Perception", "Survival"]],
      ["Vigor", ["Athletics", "Melee"]],
      ["social", ["Deception", "Diplomacy", "Performance"]],
    ]),
);
const scores = computed<Map<string, number>>(
  () =>
    new Map([
      ["Dexterity", character.creation.attributes.dexterity],
      ["Health", character.creation.attributes.health],
      ["Intellect", character.creation.attributes.intellect],
      ["Senses", character.creation.attributes.senses],
      ["Vigor", character.creation.attributes.vigor],
    ]),
);
const talents = computed<Map<Skill, number>>(() => {
  const talents: Map<Skill, number> = new Map();
  character.creation.talents.forEach(({ talent }) => {
    if (talent.skill) {
      const count: number = talents.get(talent.skill) ?? 0;
      talents.set(talent.skill, count + 1);
    }
  });
  return talents;
});

const learning = computed<number>(() => Math.max(5 + character.creation.attributes.intellect, 5));
const spent = computed<number>(
  () => [...ranks.value.values()].reduce((sum, rank) => sum + rank, 0) - [...talents.value.values()].reduce((sum, count) => sum + count, 0),
);
const remaining = computed<number>(() => learning.value - spent.value);

type SkillData = {
  key: Skill;
  name: string;
  rank: number;
  talents: number;
};
function composeSkill(key: Skill): SkillData {
  return {
    key,
    name: t(`game.skill.options.${key}`),
    rank: ranks.value.get(key) ?? 0,
    talents: talents.value.get(key) ?? 0,
  };
}

function getScore(key: string): string {
  if (key === "social") {
    return "";
  }
  const score: number = scores.value.get(key) ?? 0;
  const formatted: string = n(score, "integer");
  return score > 0 ? `+${formatted}` : formatted.replace("-", "−");
}

type AttributeData = {
  key: string;
  name: string;
  score: string;
  skills: SkillData[];
};
function composeAttribute(key: string): AttributeData {
  const score: string = getScore(key);
  return {
    key,
    name: key === "social" ? t("characters.social") : t(`game.attribute.options.${key}`),
    score,
    skills: orderBy((skills.value.get(key) ?? []).map(composeSkill), "name"),
  };
}

const attributes = computed<AttributeData[]>(() => {
  const attributes: AttributeData[] = orderBy(["Dexterity", "Health", "Intellect", "Senses", "Vigor"].map(composeAttribute), "name");
  attributes.push(composeAttribute("social"));
  return attributes;
});
const canSubmit = computed<boolean>(() => remaining.value >= 0);

function updateRank(skill: Skill, rank: number): void {
  ranks.value.set(skill, rank);
}

const { handleSubmit } = useForm();
function submit(): void {
  if (canSubmit.value) {
    const skillRanks: SkillRankPayload[] = [];
    for (const [skill, rank] of ranks.value) {
      const skillRank: SkillRankPayload = {
        skill,
        rank: rank - (talents.value.get(skill) ?? 0),
      };
      if (skillRank.rank) {
        skillRanks.push(skillRank);
      }
    }
    character.saveSkills(skillRanks);
  }
}

onMounted(() => {
  character.creation.skills.forEach((item) => ranks.value.set(item.skill, item.rank));
  for (const [skill, count] of talents.value) {
    let rank: number = ranks.value.get(skill) ?? 0;
    rank += count;
    ranks.value.set(skill, rank);
  }
});
</script>
