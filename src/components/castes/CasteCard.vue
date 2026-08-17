<template>
  <TarCard :class="classes" :subtitle="caste.feature?.name">
    <template #title-override>
      <div class="d-flex justify-content-between align-items-start gap-3 w-100">
        <h5 class="card-title">{{ caste.name }}</h5>
        <font-awesome-icon v-if="icon" :icon="icon" />
      </div>
    </template>
    <div v-if="caste.skill || caste.wealthRoll" class="card-text d-flex justify-content-between align-items-center gap-2 mb-2">
      <div class="text-start">
        <template v-if="skill"><font-awesome-icon icon="fas fa-kitchen-set" aria-hidden="true" />&nbsp;{{ skill }}</template>
      </div>
      <div v-if="caste.wealthRoll" class="text-end"><font-awesome-icon icon="fas fa-coins" aria-hidden="true" />&nbsp;{{ caste.wealthRoll }}</div>
    </div>
    <div v-if="caste.summary" class="card-text">{{ caste.summary }}</div>
    <slot></slot>
  </TarCard>
</template>

<script setup lang="ts">
import { computed } from "vue";
import { parsingUtils } from "logitar-js";
import { useI18n } from "vue-i18n";

import TarCard from "@/components/tar/TarCard.vue";
import type { Caste } from "@/types/castes";
import type { SelectionKind } from "@/types/shared";

const { parseBoolean } = parsingUtils;
const { t } = useI18n();

const props = defineProps<{
  caste: Caste;
  clickable?: boolean | string;
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

const skill = computed<string>(() => (props.caste.skill ? t(`game.skill.options.${props.caste.skill}`) : ""));
</script>
