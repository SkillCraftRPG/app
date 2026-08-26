<template>
  <div>
    <TarButton :disabled="!character.points.skills" icon="fas fa-arrow-turn-up" size="small" :text="increaseText" @click="open" />
    <TarModal centered :close="t('actions.close')" fade scrollable size="x-large" ref="modal" :title="t('characters.skills.increase')">
      <div class="row text-center mb-3">
        <div class="col">
          <div class="fw-bold">{{ t("characters.skills.available") }}</div>
          <div>{{ n(character.points.skills, "integer") }}</div>
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
        <div class="d-flex justify-content-between gap-2 mb-2">
          <div class="fs-5">{{ attribute.name }}</div>
          <div v-if="typeof attribute.score === 'number'" class="fs-5">{{ formatSignedInteger(attribute.score, n) }}</div>
        </div>
        <div class="row g-2">
          <div v-for="skill in attribute.skills" :key="skill.key" class="col-sm-6 col-lg-4 col-xl-fifth">
            <div class="border rounded p-2">
              <div class="d-flex justify-content-between gap-2 mb-1">
                <div class="text-body-secondary">{{ skill.name }}</div>
                <div class="fw-semibold" :class="{ 'text-primary': skill.isIncreased }">{{ formatSignedInteger(skill.total, n) }}</div>
              </div>
              <div class="d-flex justify-content-between align-items-end gap-2">
                <div>
                  <TarButton :disabled="!skill.increase" icon="fas fa-minus" outline size="small" variant="secondary" @click="decrease(skill.key)" />
                  <span class="mx-2">{{ n(skill.talents + skill.rank + skill.increase, "integer") }}</span>
                  <TarButton :disabled="!skill.canIncrease" icon="fas fa-plus" outline size="small" @click="increase(skill.key)" />
                </div>
                <div>
                  <TrainedSkillBadge v-if="skill.talents" />
                </div>
              </div>
            </div>
          </div>
        </div>
      </TarCard>
      <template #footer>
        <TarButton icon="fas fa-ban" :text="t('actions.cancel')" variant="secondary" @click="cancel" />
        <TarButton :disabled="!spent" icon="fas fa-arrow-turn-up" :loading="isLoading" :status="t('loading')" :text="t('actions.increase')" @click="submit" />
      </template>
    </TarModal>
  </div>
</template>

<script setup lang="ts">
import { arrayUtils } from "logitar-js";
import { computed, ref } from "vue";
import { useI18n } from "vue-i18n";

import TarButton from "@/components/tar/TarButton.vue";
import TarCard from "@/components/tar/TarCard.vue";
import TarModal from "@/components/tar/TarModal.vue";
import TrainedSkillBadge from "./TrainedSkillBadge.vue";
import type { Attribute, Skill } from "@/types/game";
import type { Character, CharacterSkill } from "@/types/characters";
import { ATTRIBUTES } from "@/types/game";
import { calculateSkill, getAttributeSkills } from "@/utils/game";
import { formatSignedInteger } from "@/utils/format";
import { getAttributeTotals, getSkillMap } from "@/utils/character";

const MAXIMUM_RANK: number[] = [2, 5, 9, 14];
const { n, t } = useI18n();
const { orderBy } = arrayUtils;

const props = defineProps<{
  character: Character;
}>();

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "updated", value: Character): void;
}>();

const increases = ref<Map<Skill, number>>(new Map());
const isLoading = ref<boolean>(false);
const modal = ref<InstanceType<typeof TarModal> | null>(null);

const increaseText = computed<string>(() => `${t("actions.increase")} (${formatSignedInteger(props.character.points.skills, n)})`);
const maximumRank = computed<number>(() => MAXIMUM_RANK[props.character.tier] ?? 0);
const spent = computed<number>(() => [...increases.value.values()].reduce((total, increase) => total + increase, 0));
const remaining = computed<number>(() => props.character.points.skills - spent.value);

const attributeMap = computed<Map<Attribute, number>>(() => getAttributeTotals(props.character));
const skillMap = computed<Map<Skill, CharacterSkill>>(() => getSkillMap(props.character));
const talents = computed<Map<Skill, number>>(() => {
  const map: Map<Skill, number> = new Map();
  props.character.talents.forEach((acquired) => {
    if (acquired.talent.skill) {
      const talents: number = map.get(acquired.talent.skill) ?? 0;
      map.set(acquired.talent.skill, talents + 1);
    }
  });
  return map;
});

type SkillData = {
  key: Skill;
  name: string;
  talents: number;
  rank: number;
  increase: number;
  total: number;
  isIncreased: boolean;
  canIncrease: boolean;
};
function produceSkillData(skill: Skill): SkillData {
  const talentCount: number = talents.value.get(skill) ?? 0;
  const data: CharacterSkill | undefined = skillMap.value.get(skill);
  const rank: number = data?.rank ?? 0;
  const increase: number = increases.value.get(skill) ?? 0;
  const total: number = calculateSkill(skill, attributeMap.value, props.character.talents, rank + increase, props.character.modifiers);
  return {
    key: skill,
    name: t(`game.skill.options.${skill}`),
    talents: talentCount,
    rank,
    increase,
    total,
    isIncreased: (data?.total ?? 0) !== total,
    canIncrease: Boolean(remaining.value && talentCount + rank + increase < maximumRank.value),
  };
}

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
      score: attributeMap.value.get(attribute) ?? 0,
      skills: orderBy(getAttributeSkills(attribute).map(produceSkillData), "name"),
    })),
    "name",
  );
  attributes.push({
    key: "Social",
    name: t("characters.social"),
    skills: orderBy(getAttributeSkills("Social").map(produceSkillData), "name"),
  });
  return attributes;
});

function decrease(skill: Skill): void {
  const increase: number = increases.value.get(skill) ?? 0;
  increases.value.set(skill, increase - 1);
}
function increase(skill: Skill): void {
  const increase: number = increases.value.get(skill) ?? 0;
  increases.value.set(skill, increase + 1);
}

function reset(): void {
  increases.value.clear();
}

async function submit(): Promise<void> {
  if (!isLoading.value) {
    isLoading.value = true;
    try {
      // TODO(fpion): implement
      reset();
      modal.value?.hide();
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
