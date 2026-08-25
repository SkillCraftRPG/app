<template>
  <CasteCard :caste="caste" :clickable="isClickable" :selected="selected" :selection="selection">
    <div class="mt-2 text-end">
      <ImportStatusDisplay :status="status" />
    </div>
  </CasteCard>
</template>

<script setup lang="ts">
import { computed } from "vue";

import CasteCard from "@/components/castes/CasteCard.vue";
import ImportStatusDisplay from "@/components/import/ImportStatusDisplay.vue";
import type { Caste } from "@/types/castes";
import type { ImportStatus } from "@/types/import";
import type { SelectionKind } from "@/types/shared";

const props = defineProps<{
  caste: Caste;
  selected: boolean | string;
  status: ImportStatus;
}>();

const isClickable = computed<boolean>(() => props.status !== "UpToDate");
const selection = computed<SelectionKind | undefined>(() => (isClickable.value ? "multiple" : undefined));
</script>
