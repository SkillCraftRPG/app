<script setup lang="ts">
import { TarButton, TarCard, TarModal } from "logitar-vue3-ui";
import { computed, ref } from "vue";
import { useI18n } from "vue-i18n";

import type { BonusModel, CharacterModel } from "@/types/characters";
import type { Statistic } from "@/types/game";

const { t } = useI18n();

const props = defineProps<{
  character: CharacterModel;
  statistic: Statistic;
  text: string;
}>();

const modalRef = ref<InstanceType<typeof TarModal> | null>(null);

const base = computed<number>(() => {
  switch (props.statistic) {
    case "Constitution":
      return props.character.statistics.constitution.base;
    case "Initiative":
      return props.character.statistics.initiative.base;
    case "Learning":
      return props.character.statistics.learning.base;
    case "Power":
      return props.character.statistics.power.base;
    case "Precision":
      return props.character.statistics.precision.base;
    case "Reputation":
      return props.character.statistics.reputation.base;
    case "Strength":
      return props.character.statistics.strength.base;
    default:
      throw new Error(`The statistic '${props.statistic}' is not supported.`);
  }
});
const bonuses = computed<BonusModel[]>(() => props.character.bonuses.filter(({ category, target }) => category === "Statistic" && target === props.statistic));
const increment = computed<number>(() => {
  switch (props.statistic) {
    case "Constitution":
      return props.character.statistics.constitution.increment;
    case "Initiative":
      return props.character.statistics.initiative.increment;
    case "Learning":
      return props.character.statistics.learning.increment;
    case "Power":
      return props.character.statistics.power.increment;
    case "Precision":
      return props.character.statistics.precision.increment;
    case "Reputation":
      return props.character.statistics.reputation.increment;
    case "Strength":
      return props.character.statistics.strength.increment;
    default:
      throw new Error(`The statistic '${props.statistic}' is not supported.`);
  }
});
const levelUps = computed<number[]>(() =>
  props.character.levelUps.reduce((values, levelUp) => {
    switch (props.statistic) {
      case "Constitution":
        values.push(levelUp.constitution);
        break;
      case "Initiative":
        values.push(levelUp.initiative);
        break;
      case "Learning":
        values.push(levelUp.learning);
        break;
      case "Power":
        values.push(levelUp.power);
        break;
      case "Precision":
        values.push(levelUp.precision);
        break;
      case "Reputation":
        values.push(levelUp.reputation);
        break;
      case "Strength":
        values.push(levelUp.strength);
        break;
      default:
        throw new Error(`The statistic '${props.statistic}' is not supported.`);
    }
    return values;
  }, [] as number[]),
);
const total = computed<number>(() => {
  switch (props.statistic) {
    case "Constitution":
      return props.character.statistics.constitution.value;
    case "Initiative":
      return props.character.statistics.initiative.value;
    case "Learning":
      return props.character.statistics.learning.value;
    case "Power":
      return props.character.statistics.power.value;
    case "Precision":
      return props.character.statistics.precision.value;
    case "Reputation":
      return props.character.statistics.reputation.value;
    case "Strength":
      return props.character.statistics.strength.value;
    default:
      throw new Error(`The statistic '${props.statistic}' is not supported.`);
  }
});

function hide(): void {
  modalRef.value?.hide();
}
</script>

<template>
  <span>
    <TarCard class="clickable" :title="text" data-bs-toggle="modal" :data-bs-target="`#${statistic}`">
      <span>{{ total }}</span>
    </TarCard>
    <TarModal :close="t('actions.close')" :id="statistic" ref="modalRef" :title="text">
      <div>{{ t("characters.statistics.baseFormat", { base }) }}</div>
      <div v-for="(increment, index) in levelUps" :key="index">{{ t("characters.level.format", { level: index + 1 }) }} (+{{ increment }})</div>
      <div v-for="bonus in bonuses" :key="bonus.id">
        {{ bonus.precision ?? t("characters.bonus") }} ({{ bonus.value > 0 ? `+${bonus.value}` : bonus.value }})
      </div>
      <div class="my-3">
        <strong>{{ t("characters.statistics.totalFormat", { increment, total }) }}</strong>
      </div>
      <template #footer>
        <TarButton icon="fas fa-times" :text="t('actions.close')" variant="secondary" @click="hide" />
      </template>
    </TarModal>
  </span>
</template>

<style scoped>
.clickable:hover {
  background-color: var(--bs-secondary-bg);
  cursor: pointer;
}
</style>
