<template>
  <LineageCard :clickable="isClickable" :lineage="lineage" :selected="selected" :selection="selection">
    <div class="mt-2 text-end">
      <ImportStatusDisplay :status="status" />
    </div>
  </LineageCard>
</template>

<script setup lang="ts">
import { computed } from "vue";

import ImportStatusDisplay from "@/components/import/ImportStatusDisplay.vue";
import LineageCard from "@/components/lineages/LineageCard.vue";
import type { ImportStatus } from "@/types/import";
import type { Lineage } from "@/types/lineages";
import type { SelectionKind } from "@/types/shared";

const props = defineProps<{
  lineage: Lineage;
  selected: boolean | string;
  status: ImportStatus;
}>();

const isClickable = computed<boolean>(() => props.status !== "UpToDate");
const selection = computed<SelectionKind | undefined>(() => (isClickable.value ? "multiple" : undefined));
</script>
