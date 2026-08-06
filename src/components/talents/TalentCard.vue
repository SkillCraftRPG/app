<template>
  <TarCard :class="classes">
    <template #title-override>
      <div class="d-flex justify-content-between align-items-start gap-3 w-100">
        <div class="d-flex flex-wrap align-items-center gap-2">
          <h5 class="card-title">{{ talent.name }}</h5>
          <TalentTierDisplay short :tier="talent.tier" />
        </div>
        <font-awesome-icon v-if="icon" :icon="icon" />
      </div>
    </template>
    <template v-if="talent.requiredTalent" #subtitle-override>
      <h6 class="card-subtitle mb-2 text-body-secondary">
        <font-awesome-icon icon="fas fa-code-branch" aria-hidden="true" />&nbsp;{{ talent.requiredTalent.name }}
      </h6>
    </template>
    <div v-if="skill" class="mb-2"><font-awesome-icon icon="fas fa-kitchen-set" aria-hidden="true" />&nbsp;{{ skill }}</div>
    <div v-else-if="talent.allowMultiplePurchases" class="mb-2">
      <TarBadge pill variant="secondary">
        <font-awesome-icon icon="fas fa-tag" aria-hidden="true" />&nbsp;{{ t("talents.allowMultiplePurchases.label") }}
      </TarBadge>
    </div>
    <div v-if="talent.summary" class="card-text">{{ talent.summary }}</div>
    <slot></slot>
  </TarCard>
</template>

<script setup lang="ts">
import { computed } from "vue";
import { parsingUtils } from "logitar-js";
import { useI18n } from "vue-i18n";

import TalentTierDisplay from "./TalentTierDisplay.vue";
import TarBadge from "@/components/tar/TarBadge.vue";
import TarCard from "@/components/tar/TarCard.vue";
import type { SelectionKind } from "@/types/shared";
import type { Talent } from "@/types/talents";

const { parseBoolean } = parsingUtils;
const { t } = useI18n();

const props = defineProps<{
  clickable?: boolean | string;
  selected?: boolean | string;
  selection?: SelectionKind;
  talent: Talent;
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

const skill = computed<string>(() => (props.talent.skill ? t(`game.skill.options.${props.talent.skill}`) : ""));
</script>
