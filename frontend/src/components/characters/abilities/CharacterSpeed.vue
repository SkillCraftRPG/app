<script setup lang="ts">
import { TarButton, TarCard, TarModal } from "logitar-vue3-ui";
import { computed, ref } from "vue";
import { useI18n } from "vue-i18n";
import { parsingUtils } from "logitar-vue3-ui";

import LineageLink from "@/components/lineages/LineageLink.vue";
import type { BonusModel, CharacterModel } from "@/types/characters";
import type { LineageModel } from "@/types/lineages";
import type { Speed } from "@/types/game";

type LineageSpeed = {
  lineage: LineageModel;
  speed: number;
};

const { parseNumber } = parsingUtils;
const { t } = useI18n();

const props = defineProps<{
  character: CharacterModel;
  speed: Speed;
  value: number | string;
}>();

const modalRef = ref<InstanceType<typeof TarModal> | null>(null);

const bonuses = computed<BonusModel[]>(() => props.character.bonuses.filter(({ category, target }) => category === "Speed" && target === props.speed));
const nation = computed<LineageSpeed | undefined>(() => {
  const lineage: LineageModel | undefined = props.character.lineage.species ? props.character.lineage : undefined;
  if (!lineage) {
    return undefined;
  }
  switch (props.speed) {
    case "Burrow":
      return { lineage, speed: lineage.speeds.burrow };
    case "Climb":
      return { lineage, speed: lineage.speeds.climb };
    case "Fly":
      return { lineage, speed: lineage.speeds.fly };
    case "Hover":
      return { lineage, speed: lineage.speeds.hover };
    case "Swim":
      return { lineage, speed: lineage.speeds.swim };
    case "Walk":
      return { lineage, speed: lineage.speeds.walk };
    default:
      throw new Error(`The speed '${props.speed}' is not supported.`);
  }
});
const species = computed<LineageSpeed>(() => {
  const lineage: LineageModel = props.character.lineage.species ?? props.character.lineage;
  switch (props.speed) {
    case "Burrow":
      return { lineage, speed: lineage.speeds.burrow };
    case "Climb":
      return { lineage, speed: lineage.speeds.climb };
    case "Fly":
      return { lineage, speed: lineage.speeds.fly };
    case "Hover":
      return { lineage, speed: lineage.speeds.hover };
    case "Swim":
      return { lineage, speed: lineage.speeds.swim };
    case "Walk":
      return { lineage, speed: lineage.speeds.walk };
    default:
      throw new Error(`The speed '${props.speed}' is not supported.`);
  }
});
const text = computed<string>(() => t(`game.speed.options.${props.speed}`));
const total = computed<number>(() => parseNumber(props.value) ?? 0);

function hide(): void {
  modalRef.value?.hide();
}
</script>

<template>
  <span>
    <TarCard class="clickable" :title="text" data-bs-toggle="modal" :data-bs-target="`#${speed}`">
      <span>{{ total }} {{ t("game.units.square", { n: total }) }}</span>
    </TarCard>
    <TarModal :close="t('actions.close')" :id="speed" ref="modalRef" :title="text">
      <div v-if="!nation || species.speed > nation.speed"><LineageLink :lineage="species.lineage" /> ({{ species.speed }})</div>
      <div v-else-if="nation && nation.speed >= species.speed"><LineageLink :lineage="nation.lineage" /> ({{ nation.speed }})</div>
      <div v-for="bonus in bonuses" :key="bonus.id">
        {{ bonus.precision ?? t("characters.bonus") }} ({{ bonus.value > 0 ? `+${bonus.value}` : bonus.value }})
      </div>
      <div class="my-3">
        <strong>{{ t("characters.speeds.totalFormat", { total }) }} {{ t("game.units.square", { n: total }) }}</strong>
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
