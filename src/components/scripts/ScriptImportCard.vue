<template>
  <ScriptCard :clickable="isClickable" :script="script" :selected="selected" :selection="selection">
    <div class="mt-2 text-end">
      <ImportStatusDisplay :status="status" />
    </div>
  </ScriptCard>
</template>

<script setup lang="ts">
import { computed } from "vue";

import ImportStatusDisplay from "@/components/import/ImportStatusDisplay.vue";
import ScriptCard from "@/components/scripts/ScriptCard.vue";
import type { ImportStatus } from "@/types/import";
import type { Script } from "@/types/scripts";
import type { SelectionKind } from "@/types/shared";

const props = defineProps<{
  script: Script;
  selected: boolean | string;
  status: ImportStatus;
}>();

const isClickable = computed<boolean>(() => props.status !== "UpToDate");
const selection = computed<SelectionKind | undefined>(() => (isClickable.value ? "multiple" : undefined));
</script>
