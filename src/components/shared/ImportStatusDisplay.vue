<template>
  <TarBadge pill :variant="variant"><font-awesome-icon :icon="icon" aria-hidden="true" />&nbsp;{{ text }}</TarBadge>
</template>

<script setup lang="ts">
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import TarBadge from "@/components/tar/TarBadge.vue";
import type { BadgeVariant } from "@/types/tar/badge";
import type { ImportStatus } from "@/types/import";

const { t } = useI18n();

const props = defineProps<{
  status: ImportStatus;
}>();

const icon = computed<string>(() => {
  switch (props.status) {
    case "Outdated":
      return "fas fa-circle-arrow-up";
    case "UpToDate":
      return "fas fa-circle-check";
    default:
      return "fas fa-circle-plus";
  }
});
const text = computed<string>(() => t(`import.status.${props.status}`));
const variant = computed<BadgeVariant>(() => {
  switch (props.status) {
    case "Outdated":
      return "warning";
    case "UpToDate":
      return "success";
    default:
      return "secondary";
  }
});
</script>
