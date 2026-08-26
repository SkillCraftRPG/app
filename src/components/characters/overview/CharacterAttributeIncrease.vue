<template>
  <div>
    <TarButton :disabled="!character.points.attributes" icon="fas fa-arrow-turn-up" size="small" :text="increaseText" @click="open" />
    <TarModal centered :close="t('actions.close')" fade scrollable size="large" ref="modal" :title="t('characters.attributes.increase')">
      <div class="fw-semibold mb-3" :class="{ 'text-primary': remaining, 'text-body-secondary': !remaining }">
        {{ t("characters.attributes.remaining", { remaining }) }}
      </div>
      <section v-for="attribute in attributes" :key="attribute.key" class="mb-3">
        <div class="d-flex justify-content-between gap-2 mb-1">
          <div class="fs-5">{{ attribute.name }}</div>
          <div class="d-flex justify-content-between gap-2">
            <TarButton :disabled="!attribute.increase" icon="fas fa-minus" outline size="small" variant="secondary" @click="decrease(attribute.key)" />
            <div class="fs-5">{{ formatSignedInteger(attribute.total, n) }}</div>
            <TarButton :disabled="!attribute.canIncrease" icon="fas fa-plus" outline size="small" @click="increase(attribute.key)" />
          </div>
        </div>
        <div class="row">
          <div class="col-sm-6">
            <table class="table table-sm">
              <tbody>
                <tr v-for="statistic in attribute.statistics" :key="statistic.key">
                  <td class="w-third text-body-secondary">{{ statistic.name }}</td>
                  <td class="w-third text-center">{{ n(statistic.current, "integer") }}</td>
                  <td class="w-sixth text-center" :class="{ 'text-primary': statistic.delta }">{{ formatSignedInteger(statistic.delta, n) }}</td>
                  <td class="w-sixth text-end fw-semibold" :class="{ 'text-primary': statistic.delta }">{{ n(statistic.result, "integer") }}</td>
                </tr>
              </tbody>
            </table>
          </div>
          <div class="col-sm-6">
            <table class="table table-sm">
              <tbody>
                <tr v-for="skill in attribute.skills" :key="skill.key">
                  <td class="w-third text-body-secondary">{{ skill.name }}</td>
                  <td class="w-third text-center">{{ formatSignedInteger(skill.current, n) }}</td>
                  <td class="w-sixth text-center" :class="{ 'text-primary': skill.delta }">{{ formatSignedInteger(skill.delta, n) }}</td>
                  <td class="w-sixth text-end fw-semibold" :class="{ 'text-primary': skill.delta }">{{ formatSignedInteger(skill.result, n) }}</td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      </section>
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
import TarModal from "@/components/tar/TarModal.vue";
import type { Attribute, Skill, Statistic } from "@/types/game";
import type { Character, CharacterAttribute, CharacterSkill, CharacterStatistic, IncreaseCharacterAttributesPayload } from "@/types/characters";
import { ATTRIBUTES } from "@/types/game";
import { formatSignedInteger } from "@/utils/format";
import { getAttributeMap, getSkillMap, getStatisticMap } from "@/utils/character";
import { calculateStatisticNew, getAttributeSkills, getAttributeStatistics } from "@/utils/game";
import { increaseCharacterAttributes } from "@/api/characters";

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

const increases = ref<Map<Attribute, number>>(new Map());
const isLoading = ref<boolean>(false);
const modal = ref<InstanceType<typeof TarModal> | null>(null);

const increaseText = computed<string>(() => `${t("actions.increase")} (${formatSignedInteger(props.character.points.attributes, n)})`);
const spent = computed<number>(() => [...increases.value.values()].reduce((spent, increase) => spent + increase, 0));
const remaining = computed<number>(() => props.character.points.attributes - spent.value);

const attributeMap = computed<Map<Attribute, CharacterAttribute>>(() => getAttributeMap(props.character));
const increasedAttributes = computed<Map<Attribute, number>>(() => {
  const map: Map<Attribute, number> = new Map();
  for (const [attribute, value] of attributeMap.value) {
    const total: number = value.total + (increases.value.get(attribute) ?? 0);
    map.set(attribute, total);
  }
  return map;
});
const statisticMap = computed<Map<Statistic, CharacterStatistic>>(() => getStatisticMap(props.character));
const skillMap = computed<Map<Skill, CharacterSkill>>(() => getSkillMap(props.character));

type StatisticData = {
  key: Statistic;
  name: string;
  current: number;
  delta: number;
  result: number;
};
type SkillData = {
  key: Skill;
  name: string;
  current: number;
  delta: number;
  result: number;
};
type AttributeData = {
  key: Attribute;
  name: string;
  increase: number;
  total: number;
  canIncrease: boolean;
  statistics: StatisticData[];
  skills: SkillData[];
};
const attributes = computed<AttributeData[]>(() =>
  orderBy(
    ATTRIBUTES.map((attribute) => {
      const data: CharacterAttribute | undefined = attributeMap.value.get(attribute);
      const increase: number = increases.value.get(attribute) ?? 0;
      return {
        key: attribute,
        name: t(`game.attribute.options.${attribute}`),
        increase,
        total: (data?.total ?? 0) + increase,
        canIncrease: Boolean(remaining.value && data && data.starting + data.progression + increase < MAXIMUM_SCORE),
        statistics: orderBy(
          getAttributeStatistics(attribute).map((statistic) => {
            const current: number = statisticMap.value.get(statistic)?.total ?? 0;
            const result: number = calculateStatisticNew(statistic, increasedAttributes.value, props.character.level, props.character.modifiers);
            return {
              key: statistic,
              name: t(`game.statistic.options.${statistic}`),
              current,
              delta: result - current,
              result,
            };
          }),
          "name",
        ),
        skills: orderBy(
          getAttributeSkills(attribute).map((skill) => {
            const current: number = skillMap.value.get(skill)?.total ?? 0;
            return {
              key: skill,
              name: t(`game.skill.options.${skill}`),
              current,
              delta: increase,
              result: current + increase,
            };
          }),
          "name",
        ),
      };
    }),
    "name",
  ),
);

function decrease(attribute: Attribute): void {
  const increase: number = increases.value.get(attribute) ?? 0;
  increases.value.set(attribute, increase - 1);
}
function increase(attribute: Attribute): void {
  const increase: number = increases.value.get(attribute) ?? 0;
  increases.value.set(attribute, increase + 1);
}

function reset(): void {
  increases.value.clear();
}

async function submit(): Promise<void> {
  if (!isLoading.value) {
    isLoading.value = true;
    try {
      const payload: IncreaseCharacterAttributesPayload = {
        dexterity: increases.value.get("Dexterity") ?? 0,
        health: increases.value.get("Health") ?? 0,
        intellect: increases.value.get("Intellect") ?? 0,
        senses: increases.value.get("Senses") ?? 0,
        vigor: increases.value.get("Vigor") ?? 0,
      };
      const character: Character = await increaseCharacterAttributes(props.character.id, payload);
      emit("updated", character);
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
