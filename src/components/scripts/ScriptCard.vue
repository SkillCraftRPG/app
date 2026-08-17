<template>
  <TarCard :class="classes">
    <template #title-override>
      <div class="d-flex justify-content-between align-items-start gap-3 w-100">
        <h5 class="card-title">{{ script.name }}</h5>
        <font-awesome-icon v-if="icon" :icon="icon" />
      </div>
    </template>
    <div v-if="script.summary" class="card-text">{{ script.summary }}</div>
    <slot></slot>
  </TarCard>
</template>

<script setup lang="ts">
import { computed } from "vue";
import { parsingUtils } from "logitar-js";

import TarCard from "@/components/tar/TarCard.vue";
import type { Script } from "@/types/scripts";
import type { SelectionKind } from "@/types/shared";

const { parseBoolean } = parsingUtils;

const props = defineProps<{
  clickable?: boolean | string;
  script: Script;
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
</script>
