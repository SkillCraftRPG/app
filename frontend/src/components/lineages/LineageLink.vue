<script setup lang="ts">
import type { LineageModel } from "@/types/lineages";

import LineageIcon from "./LineageIcon.vue";

defineProps<{
  lineage: LineageModel;
}>();
</script>

<template>
  <span>
    <RouterLink :to="{ name: 'LineageEdit', params: { id: lineage.id } }" target="_blank"><LineageIcon /></RouterLink>
    {{ " " }}
    <RouterLink :to="{ name: 'LineageEdit', params: { id: lineage.id } }" target="_blank">{{ lineage.name }}</RouterLink>
  </span>
</template>
