<template>
  <ScriptCard :clickable="isClickable" :script="script" :selected="selected">
    <div class="d-flex justify-content-between mt-2">
      <div>
        <font-awesome-icon v-if="icon" :icon="icon" />
      </div>
      <div>
        <ImportStatusDisplay :status="status" />
      </div>
    </div>
  </ScriptCard>
</template>

<script setup lang="ts">
import { computed } from "vue";
import { parsingUtils } from "logitar-js";

import ImportStatusDisplay from "@/components/import/ImportStatusDisplay.vue";
import ScriptCard from "@/components/scripts/ScriptCard.vue";
import type { ImportStatus } from "@/types/import";
import type { Script } from "@/types/scripts";

const { parseBoolean } = parsingUtils;

const props = defineProps<{
  script: Script;
  selected: boolean | string;
  status: ImportStatus;
}>();

const isClickable = computed<boolean>(() => props.status !== "UpToDate");
const icon = computed<string | undefined>(() => {
  if (isClickable.value) {
    return parseBoolean(props.selected) ? "far fa-square-check" : "far fa-square";
  }
});
</script>
