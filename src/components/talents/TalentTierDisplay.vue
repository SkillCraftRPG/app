<template>
  <TarBadge :pill="isShort" variant="secondary">
    <template v-if="!isShort">{{ t("talents.tier.label") }}&nbsp;</template>{{ tier }}
  </TarBadge>
</template>

<script setup lang="ts">
import { computed } from "vue";
import { parsingUtils } from "logitar-js";
import { useI18n } from "vue-i18n";

import TarBadge from "@/components/tar/TarBadge.vue";

const { parseBoolean } = parsingUtils;
const { t } = useI18n();

const props = defineProps<{
  short?: boolean | string;
  tier: number;
}>();

const isShort = computed<boolean>(() => parseBoolean(props.short) ?? false);
</script>
