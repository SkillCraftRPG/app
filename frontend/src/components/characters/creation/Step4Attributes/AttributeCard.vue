<script setup lang="ts">
import { TarCard } from "logitar-vue3-ui";
import { computed } from "vue";
import { parsingUtils } from "logitar-js";
import { useI18n } from "vue-i18n";

import type { Attribute } from "@/types/game";

const { parseBoolean } = parsingUtils;
const { t } = useI18n();

const props = defineProps<{
  attribute: Attribute;
  selected?: boolean | string;
}>();

const isSelected = computed<boolean>(() => parseBoolean(props.selected) ?? false);

defineEmits<{
  (e: "click"): void;
}>();
</script>

<template>
  <TarCard :class="{ 'clickable h-100': true, selected: isSelected }" :title="t(`game.attributes.options.${attribute}`)" @click="$emit('click')">
    <template v-if="isSelected" #subtitle-override>
      <h6 class="card-subtitle mb-2 text-body-secondary"><font-awesome-icon icon="fas fa-check" /> {{ t("characters.attributes.selected") }}</h6>
    </template>
  </TarCard>
</template>

<style scoped>
.clickable.selected {
  background-color: var(--bs-tertiary-bg);
}
.clickable:hover {
  background-color: var(--bs-secondary-bg);
  cursor: pointer;
}
</style>
