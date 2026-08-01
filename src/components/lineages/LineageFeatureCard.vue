<template>
  <TarCard :title="feature.name">
    <div v-if="summary" class="card-text">{{ summary }}</div>
    <slot>
      <StatusBlock :actor="feature.updatedBy" class="card-text mt-2 small text-secondary" :date="feature.updatedOn" relative />
    </slot>
  </TarCard>
</template>

<script setup lang="ts">
import { computed } from "vue";
import { stringUtils } from "logitar-js";

import StatusBlock from "@/components/shared/StatusBlock.vue";
import TarCard from "@/components/tar/TarCard.vue";
import type { LineageFeature } from "@/types/lineages";

const { shortify } = stringUtils;

const props = defineProps<{
  feature: LineageFeature;
}>();

const summary = computed<string>(() => {
  if (!props.feature.content) {
    return "";
  }
  const lines: string[] = props.feature.content.split("\n");
  return shortify(lines[0] ?? "", 72);
});
</script>
