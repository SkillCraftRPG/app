<template>
  <span class="kind"
    ><template v-if="icon"><font-awesome-icon v-if="icon" :icon="icon" />&nbsp;</template>{{ text }}</span
  >
</template>

<script setup lang="ts">
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import type { CustomizationKind } from "@/types/customizations";

const { t } = useI18n();

const props = defineProps<{
  kind: CustomizationKind;
}>();

const icon = computed<string | undefined>(() => {
  switch (props.kind) {
    case "Disability":
      return "fas fa-wheelchair";
    case "Gift":
      return "fas fa-trophy";
  }
});
const text = computed<string>(() => t(`customizations.kind.options.${props.kind}`));
</script>
