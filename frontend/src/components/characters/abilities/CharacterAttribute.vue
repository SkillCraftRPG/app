<script setup lang="ts">
import { TarButton, TarCard, TarModal } from "logitar-vue3-ui";
import { computed, ref } from "vue";
import { useI18n } from "vue-i18n";

import LineageLink from "@/components/lineages/LineageLink.vue";
import NatureLink from "@/components/natures/NatureLink.vue";
import type { Attribute } from "@/types/game";
import type { BonusModel, CharacterModel } from "@/types/characters";
import type { LineageModel } from "@/types/lineages";

type LineageBonus = {
  bonus: number;
  lineage: LineageModel;
};

const { t } = useI18n();

const props = defineProps<{
  attribute: Attribute;
  character: CharacterModel;
  modifier: number;
  score: number;
  text: string;
}>();

const modalRef = ref<InstanceType<typeof TarModal> | null>(null);

const base = computed<number>(() => {
  switch (props.attribute) {
    case "Agility":
      return props.character.baseAttributes.agility;
    case "Coordination":
      return props.character.baseAttributes.coordination;
    case "Intellect":
      return props.character.baseAttributes.intellect;
    case "Presence":
      return props.character.baseAttributes.presence;
    case "Sensitivity":
      return props.character.baseAttributes.sensitivity;
    case "Spirit":
      return props.character.baseAttributes.spirit;
    case "Vigor":
      return props.character.baseAttributes.vigor;
    default:
      throw new Error(`The attribute '${props.attribute}' is not supported.`);
  }
});
const bonuses = computed<BonusModel[]>(() => props.character.bonuses.filter(({ category, target }) => category === "Attribute" && target === props.attribute));
const extra = computed<Attribute[]>(() => props.character.baseAttributes.extra.filter((attribute) => attribute === props.attribute));
const formattedModifier = computed<string>(() => (props.modifier > 0 ? `+${props.modifier}` : props.modifier).toString());
const levels = computed<number[]>(() => {
  const levels: number[] = [];
  for (let i = 0; i < props.character.levelUps.length; i++) {
    if (props.character.levelUps[i]) {
      levels.push(i + 1);
    }
  }
  return levels;
});
const mandatory = computed<Attribute[]>(() => props.character.baseAttributes.mandatory.filter((attribute) => attribute === props.attribute));
const nation = computed<LineageBonus | undefined>(() => {
  const lineage: LineageModel | undefined = props.character.lineage.species ? props.character.lineage : undefined;
  if (!lineage) {
    return undefined;
  }
  switch (props.attribute) {
    case "Agility":
      return { lineage, bonus: lineage.attributes.agility };
    case "Coordination":
      return { lineage, bonus: lineage.attributes.coordination };
    case "Intellect":
      return { lineage, bonus: lineage.attributes.intellect };
    case "Presence":
      return { lineage, bonus: lineage.attributes.presence };
    case "Sensitivity":
      return { lineage, bonus: lineage.attributes.sensitivity };
    case "Spirit":
      return { lineage, bonus: lineage.attributes.spirit };
    case "Vigor":
      return { lineage, bonus: lineage.attributes.vigor };
    default:
      throw new Error(`The attribute '${props.attribute}' is not supported.`);
  }
});
const optional = computed<Attribute[]>(() => props.character.baseAttributes.optional.filter((attribute) => attribute === props.attribute));
const species = computed<LineageBonus>(() => {
  const lineage: LineageModel = props.character.lineage.species ?? props.character.lineage;
  switch (props.attribute) {
    case "Agility":
      return { lineage, bonus: lineage.attributes.agility };
    case "Coordination":
      return { lineage, bonus: lineage.attributes.coordination };
    case "Intellect":
      return { lineage, bonus: lineage.attributes.intellect };
    case "Presence":
      return { lineage, bonus: lineage.attributes.presence };
    case "Sensitivity":
      return { lineage, bonus: lineage.attributes.sensitivity };
    case "Spirit":
      return { lineage, bonus: lineage.attributes.spirit };
    case "Vigor":
      return { lineage, bonus: lineage.attributes.vigor };
    default:
      throw new Error(`The attribute '${props.attribute}' is not supported.`);
  }
});

function hide(): void {
  modalRef.value?.hide();
}
</script>

<template>
  <span>
    <TarCard class="clickable" :title="text" data-bs-toggle="modal" :data-bs-target="`#${attribute}`">
      <span>{{ score }}</span>
      <span class="float-end">{{ formattedModifier }}</span>
    </TarCard>
    <TarModal :close="t('actions.close')" :id="attribute" ref="modalRef" :title="text">
      <div>{{ t("characters.attributes.baseFormat", { base }) }}</div>
      <div v-if="species.bonus > 0"><LineageLink :lineage="species.lineage" /> (+{{ species.bonus }})</div>
      <div v-if="nation && nation.bonus > 0"><LineageLink :lineage="nation.lineage" /> (+{{ nation.bonus }})</div>
      <div v-for="(_, index) in extra" :key="index">{{ t("characters.attributes.extra.bonus") }}</div>
      <div v-if="character.nature.attribute === attribute"><NatureLink :nature="character.nature" /> (+1)</div>
      <div v-if="character.baseAttributes.best === attribute">{{ t("characters.attributes.mandatory.best") }}</div>
      <div v-for="(_, index) in mandatory" :key="index">{{ t("characters.attributes.mandatory.mandatory") }}</div>
      <div v-if="character.baseAttributes.worst === attribute">{{ t("characters.attributes.mandatory.worst") }}</div>
      <div v-for="(_, index) in optional" :key="index">{{ t("characters.attributes.optional.bonus") }}</div>
      <div v-for="level in levels" :key="level">{{ t("characters.level.format", { level }) }}</div>
      <div v-for="bonus in bonuses" :key="bonus.id">
        {{ bonus.precision ?? t("characters.bonus") }} ({{ bonus.value > 0 ? `+${bonus.value}` : bonus.value }})
      </div>
      <div class="my-3">
        <strong>{{ t("characters.attributes.scoreFormat", { score, modifier: formattedModifier }) }}</strong>
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
