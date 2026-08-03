<template>
  <TalentCard :clickable="isClickable" :selected="selected" :talent="talent">
    <div class="d-flex justify-content-between mt-2">
      <div>
        <font-awesome-icon v-if="icon" :icon="icon" />
      </div>
      <div>
        <ImportStatusDisplay :status="status" />
      </div>
    </div>
  </TalentCard>
</template>

<script setup lang="ts">
import { computed } from "vue";
import { parsingUtils } from "logitar-js";

import ImportStatusDisplay from "@/components/import/ImportStatusDisplay.vue";
import TalentCard from "@/components/talents/TalentCard.vue";
import type { ImportStatus } from "@/types/import";
import type { Talent } from "@/types/talents";

const { parseBoolean } = parsingUtils;

const props = defineProps<{
  selected: boolean | string;
  status: ImportStatus;
  talent: Talent;
}>();

const isClickable = computed<boolean>(() => props.status !== "UpToDate");
const icon = computed<string | undefined>(() => {
  if (isClickable.value) {
    return parseBoolean(props.selected) ? "far fa-square-check" : "far fa-square";
  }
});
</script>
