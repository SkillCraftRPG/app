<template>
  <TarCard :class="classes">
    <template #title-override>
      <div class="d-flex justify-content-between align-items-start gap-3 w-100">
        <h5 class="card-title">{{ lineage.name }}</h5>
        <font-awesome-icon v-if="icon" :icon="icon" />
      </div>
    </template>
    <template #subtitle-override>
      <h6 class="card-subtitle mb-2 text-body-secondary"><font-awesome-icon icon="fas fa-ruler-vertical" aria-hidden="true" />&nbsp;{{ sizeText }}</h6>
    </template>
    <div v-if="lineage.summary" class="card-text">{{ lineage.summary }}</div>
    <slot></slot>
  </TarCard>
</template>

<script setup lang="ts">
import { computed } from "vue";
import { parsingUtils } from "logitar-js";
import { useI18n } from "vue-i18n";

import TarCard from "@/components/tar/TarCard.vue";
import type { Lineage } from "@/types/lineages";
import type { SelectionKind } from "@/types/shared";

const { parseBoolean } = parsingUtils;
const { t } = useI18n();

const props = defineProps<{
  clickable?: boolean | string;
  lineage: Lineage;
  selected?: boolean | string;
  selection?: SelectionKind;
}>();

const isSelected = computed<boolean>(() => parseBoolean(props.selected) ?? false);
const classes = computed<string[]>(() => {
  const classes: string[] = [];
  if (parseBoolean(props.clickable)) {
    classes.push("clickable");
  }
  if (isSelected.value) {
    classes.push("border-primary", "bg-primary-subtle");
  }
  return classes;
});
const icon = computed<string>(() => {
  switch (props.selection) {
    case "single":
      return isSelected.value ? "far fa-circle-check" : "far fa-circle";
    case "multiple":
      return isSelected.value ? "far fa-square-check" : "far fa-square";
    default:
      return "";
  }
});

const sizeText = computed<string>(() => t(`game.size.category.options.${props.lineage.size.category}`));
</script>
