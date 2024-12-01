<script setup lang="ts">
import { TarButton, TarCard, TarModal } from "logitar-vue3-ui";
import { computed, ref } from "vue";
import { useI18n } from "vue-i18n";

import type { BonusModel, CharacterModel } from "@/types/characters";
import type { Statistic } from "@/types/game";

const { t } = useI18n();

const props = defineProps<{
  base: number;
  character: CharacterModel;
  increment: number;
  statistic: Statistic;
  text: string;
  total: number;
}>();

const modalRef = ref<InstanceType<typeof TarModal> | null>(null);

const bonuses = computed<BonusModel[]>(() => props.character.bonuses.filter(({ category, target }) => category === "Statistic" && target === props.statistic));
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
      <div v-for="(increment, index) in levelUps" :key="index">{{ t("characters.level.format", { level: index + 1 }) }} ({{ increment }})</div>
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
