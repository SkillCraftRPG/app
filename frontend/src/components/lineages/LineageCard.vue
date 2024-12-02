<script setup lang="ts">
import { TarCard } from "logitar-vue3-ui";
import { useRouter } from "vue-router";

import type { LineageModel } from "@/types/lineages";

const router = useRouter();

const props = defineProps<{
  lineage: LineageModel;
}>();

function onClick(): void {
  router.push({ name: "LineageEdit", params: { id: props.lineage.id } });
}
</script>

<template>
  <TarCard class="clickable" :title="lineage.name" @click="onClick" />
</template>

<style scoped>
.clickable:hover {
  background-color: var(--bs-secondary-bg);
  cursor: pointer;
}
</style>
